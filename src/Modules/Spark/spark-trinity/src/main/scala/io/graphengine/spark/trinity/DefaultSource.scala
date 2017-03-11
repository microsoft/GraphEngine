// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
package io.graphengine.spark.trinity

import org.apache.spark.sql.SQLContext
import org.apache.spark.sql.sources.{BaseRelation, DataSourceRegister, RelationProvider}

class DefaultSource extends RelationProvider with DataSourceRegister {

  override def shortName(): String = "trinity"

  override def createRelation(sqlContext: SQLContext, parameters: Map[String, String]): BaseRelation = {
    val options = new TrinityOptions(parameters)
    TrinityRelation(options)(sqlContext.sparkContext, sqlContext)
  }
}
