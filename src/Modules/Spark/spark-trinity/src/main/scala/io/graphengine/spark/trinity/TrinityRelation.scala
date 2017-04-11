// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
package io.graphengine.spark.trinity

import org.apache.spark.SparkContext
import org.apache.spark.rdd.RDD
import org.apache.spark.sql.sources._
import org.apache.spark.sql.types.StructType
import org.apache.spark.sql.{Row, SQLContext}

private[spark] case class TrinityRelation(options: TrinityOptions)
                                         (@transient val sparkContext: SparkContext, val sqlContext: SQLContext)
  extends BaseRelation with PrunedFilteredScan {

  val trinityClient : TrinityClient = options.rddFormat match {
    case options.TRINITY_RDDFORMAT_JSON => new TrinityHttpJsonClient(options)
    case _ => throw new NotImplementedError(s"Unsupported trinity rdd format '${options.rddFormat}'")
  }

  override def schema: StructType = trinityClient.getSchema(options.cellType)

  override def buildScan(requiredColumns: Array[String], filters: Array[Filter]): RDD[Row] = options.rddFormat match {
    case options.TRINITY_RDDFORMAT_JSON => {
      val filteredSchema = new StructType(requiredColumns.map(col => schema.fields.find(f => f.name == col).get))
      val baseRdd = new TrinityRDD[String](
        sparkContext,
        filteredSchema,
        trinityClient.getPartitions(options.cellType, filters),
        requiredColumns,
        trinityClient.retrieveCells)

      sqlContext.read.schema(filteredSchema).json(baseRdd).rdd
    }
    case _ => throw new NotImplementedError(s"Unsupported trinity rdd format '${options.rddFormat}'")
  }
}
