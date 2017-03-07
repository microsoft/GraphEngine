// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
package io.graphengine.spark.trinity

import org.apache.spark.Partition
import org.apache.spark.sql.sources._
import org.apache.spark.sql.types._
import org.json4s.DefaultFormats
import org.json4s.JsonAST._
import org.json4s.JsonDSL._
import org.json4s.native.Json
import org.json4s.native.JsonMethods._

import scalaj.http.{Http, HttpOptions, HttpRequest}

trait TrinityClient {

  def getSchema(cellType: String): StructType

  def getPartitions(cellType: String, filters: Array[Filter]): Array[Partition]

  def retrieveCells(partition: Partition, schema: StructType, requiredColumns: Array[String]): Array[Any]
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


  def compileFilter(filter: Filter, depth: Int = 0, indent: String = "  "): Option[JObject] = {
    val indents = indent * depth
    def format(o: String, a:String, v:Any) = ("operator"->s"$o")~("attr"->s"$a")~("value"->s"$v")
    def format2(o: String, l:List[JValue]) = ("operator"->s"$o")~("filters"->l)

    Option(filter match {
      case GreaterThan(attr, value) => format("GreaterThan", attr, value)
      case LessThan(attr, value) => format("LessThan", attr, value)
      case StringStartsWith(attr, value) => format("StringStartsWith", attr, value)
      case StringEndsWith(attr, value) => format("StringEndsWith", attr, value)
      case StringContains(attr, value) => format("StringContains", attr, value)
      case And(left, right) => {
        val and = Seq(left, right).flatMap(compileFilter(_, depth+1))
        if (and.size == 2) format2("And", and.toList)
        else null
      }
      case Or(left, right) => {
        val or = Seq(left, right).flatMap(compileFilter(_, depth+1))
        if (or.size == 2) format2("Or", or.toList)
        else null
      }
      case _ => null
    })
  }

  override def getSchema(cellType: String): StructType = {
    val json = parse(post("/Spark/Schema/", s"""{"cellType":"${trinityOptions.cellType}"}""").asString.body)
    getCatalystType(json.asInstanceOf[JObject]).asInstanceOf[StructType]
  }

  override def getPartitions(cellType: String, filters: Array[Filter]): Array[Partition] = {

    val compiledFilters = filters.flatMap(compileFilter(_))

    val requestBody = compact(render(("cellType"->trinityOptions.cellType)~("batchSize"->trinityOptions.batchSize)~("filters"->compiledFilters.toList)))
    println(requestBody)
    val response = post("/Spark/Partitions/", requestBody, true).asString.body
    val partitions = response.substring(1, response.length-2).split("],")
    Array.tabulate(partitions.length) {
      i => {
        val partition = partitions(i).substring(1).split(',')
        new TrinityPartition(cellType, Array.tabulate(partition.length) {j => partition(j).toLong}, i)
      }
    }
  }

  override def retrieveCells(partition: Partition, schema: StructType, requiredColumns: Array[String]): Array[Any] = {

    val part = partition.asInstanceOf[TrinityPartition]
    val requestBody = s"""{"cellType":"${trinityOptions.cellType}", "partition": ${Json(DefaultFormats).write(part.cellIds)}}"""
    val json = parse(post("/Spark/GetPartition/", requestBody, true).asString.body)

    json.children.map(child => { implicit val formats = DefaultFormats; child.extract[String] }).toArray
  }
}
