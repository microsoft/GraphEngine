package io.graphengine.spark

import org.apache.spark.sql.SQLContext
import org.apache.spark.sql.sources.{BaseRelation, DataSourceRegister, RelationProvider}

class DefaultSource extends RelationProvider with DataSourceRegister {

  override def shortName(): String = "trinity"

  override def createRelation(sqlContext: SQLContext, parameters: Map[String, String]): BaseRelation = {
    val options = new TrinityOptions(parameters)
    TrinityRelation(options)(sqlContext.sparkContext, sqlContext)
  }
}
