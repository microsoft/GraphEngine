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

  def toSparkDataType(fieldType: String): DataType = fieldType match {
    case "Int32" => IntegerType
    case "Int64" => LongType
    case "Boolean" => BooleanType
    case "DateTime" => TimestampType
    case "String" => StringType
  }

  def post(path: String, data: String, compress: Boolean = true): HttpRequest = Http(s"${trinityOptions.serverAddress}$path")
    .option(HttpOptions.connTimeout(connTimeout))
    .option(HttpOptions.readTimeout(readTimeout))
    .header("content-type", "application/json")
    .compress(compress)
    .postData(data)

  override def getSchema(cellType: String): StructType = {
    val json = parse(post("/Spark/Schema/", s"""{"cellType":"${trinityOptions.cellType}"}""").asString.body)

    val arrayBuffer = ArrayBuffer.empty[StructField]
    for {JObject(x) <- json
         JField("name", JString(name)) <- x
         JField("dataType", JString(dataType)) <- x
         JField("nullable", JBool(nullable)) <- x
         JField("isList", JBool(isList)) <- x} {
      val t = toSparkDataType(dataType)
      val dt = if (isList) ArrayType(t, nullable) else t
      arrayBuffer += new StructField(name, dt, nullable)
    }

    StructType(arrayBuffer.toArray)
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
