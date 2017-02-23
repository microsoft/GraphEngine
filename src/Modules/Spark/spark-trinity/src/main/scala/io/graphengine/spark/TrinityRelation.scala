package io.graphengine.spark

import org.apache.spark.SparkContext
import org.apache.spark.rdd.RDD
import org.apache.spark.sql.sources.{BaseRelation, TableScan}
import org.apache.spark.sql.types.StructType
import org.apache.spark.sql.{Row, SQLContext}

private[spark] case class TrinityRelation(options: TrinityOptions)
                                         (@transient val sparkContext: SparkContext, val sqlContext: SQLContext)
  extends BaseRelation with TableScan /*with CatalystScan*/ {

  val trinityClient : TrinityClient = options.rddFormat match {
    case options.TRINITY_RDDFORMAT_JSON => new TrinityHttpJsonClient(options)
    case _ => throw new NotImplementedError(s"Unsupported trinity rdd format '${options.rddFormat}'")
  }

  override def schema: StructType = trinityClient.getSchema(options.cellType)

  override def buildScan(): RDD[Row] = options.rddFormat match {
    case options.TRINITY_RDDFORMAT_JSON => {
      val baseRdd = new TrinityRDD[String](sparkContext, schema,
        trinityClient.getPartitions(options.cellType), trinityClient.retrieveCells)
      sqlContext.read.schema(schema).json(baseRdd).rdd
    }
    case _ => throw new NotImplementedError(s"Unsupported trinity rdd format '${options.rddFormat}'")
  }
}
