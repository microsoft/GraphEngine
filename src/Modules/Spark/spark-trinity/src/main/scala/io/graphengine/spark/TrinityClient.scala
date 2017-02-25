package io.graphengine.spark

import org.apache.spark.Partition
import org.apache.spark.sql.types._
import org.json4s.DefaultFormats
import org.json4s.JsonAST._
import org.json4s.native.Json
import org.json4s.native.JsonMethods._

import scala.collection.mutable.ArrayBuffer
import scalaj.http.{Http, HttpOptions, HttpRequest}

trait TrinityClient {

  def getSchema(cellType: String): StructType

  def getPartitions(cellType: String): Array[Partition]

  def retrieveCells(partition: Partition, schema: StructType): Array[Any]
}

class TrinityHttpJsonClient(trinityOptions: TrinityOptions)
  extends TrinityClient with Serializable {

  val connTimeout = 30 * 1000 // 30 seconds
  val readTimeout = 10 * 60 * 1000 // 10 minutes

  case class TrinityPartition(cellType: String, cellIds: Array[Long], idx: Int) extends Partition {
    override def index: Int = idx
  }

  def getStructField(field: JObject): StructField = {
    implicit val formats = DefaultFormats;
    StructField(
      (field \ "name").extract[String],
      getCatalystType((field \ "type").asInstanceOf[JObject]),
      (field \ "nullable").extract[Boolean])
  }

  def getCatalystType(jObj: JObject): DataType = {
    implicit val formats = DefaultFormats;
    (jObj \ "typeName").extract[String] match {
      case "System.Byte"       => ByteType
      case "System.SByte"      => ByteType
      case "System.Boolean"    => BooleanType
      case "System.Char"       => StringType
      case "System.Int16"      => IntegerType
      case "System.UInt16"     => IntegerType
      case "System.Int32"      => IntegerType
      case "System.UInt32"     => LongType
      case "System.Int64"      => LongType
      case "System.UInt64"     => DecimalType(20, 0)
      case "System.Single"     => FloatType
      case "System.Double"     => DoubleType
      case "System.Decimal"    => DecimalType.SYSTEM_DEFAULT
      case "System.DateTime"   => TimestampType
      case "System.Guid"       => StringType
      case "System.String"     => StringType
      case "ArrayType"         => ArrayType(getCatalystType((jObj \ "elementType").asInstanceOf[JObject]))
      case "NullableValueType" => getCatalystType((jObj \ "argumentType").asInstanceOf[JObject])
      case "StructType"        => StructType((jObj \ "fields").asInstanceOf[JArray].children.map(f => getStructField(f.asInstanceOf[JObject])))
    }
  }

  def post(path: String, data: String, compress: Boolean = true): HttpRequest = Http(s"${trinityOptions.serverAddress}$path")
    .option(HttpOptions.connTimeout(connTimeout))
    .option(HttpOptions.readTimeout(readTimeout))
    .header("content-type", "application/json")
    .compress(compress)
    .postData(data)

  override def getSchema(cellType: String): StructType = {
    val json = parse(post("/Spark/Schema/", s"""{"cellType":"${trinityOptions.cellType}"}""").asString.body)
    getCatalystType(json.asInstanceOf[JObject]).asInstanceOf[StructType]
  }

  override def getPartitions(cellType: String): Array[Partition] = {
    val requestBody = s"""{"cellType":"${trinityOptions.cellType}", "batchSize": ${trinityOptions.batchSize}}"""
    val response = post("/Spark/Partitions/", requestBody, true).asString.body
    val partitions = response.substring(1, response.length-2).split("],")
    Array.tabulate(partitions.length) {
      i => {
        val partition = partitions(i).substring(1).split(',')
        new TrinityPartition(cellType, Array.tabulate(partition.length) {j => partition(j).toLong}, i)
      }
    }
  }

  override def retrieveCells(partition: Partition, schema: StructType): Array[Any] = {

    val part = partition.asInstanceOf[TrinityPartition]
    val requestBody = s"""{"cellType":"${trinityOptions.cellType}", "partition": ${Json(DefaultFormats).write(part.cellIds)}}"""
    val json = parse(post("/Spark/GetPartition/", requestBody, true).asString.body)

    json.children.map(child => { implicit val formats = DefaultFormats; child.extract[String] }).toArray
  }
}
