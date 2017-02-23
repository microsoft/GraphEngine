package io.graphengine.spark

class TrinityOptions(@transient private val parameters: Map[String, String]) extends Serializable{

  val TRINITY_SERVER = "server"
  val TRINITY_CELLTYPE = "cellType"
  val TRINITY_BATCHSIZE = "batchSize"
  val TRINITY_RDDFORMAT = "rddFormat"

  val TRINITY_BATCHSIZE_DEFAULT = 1000
  val TRINITY_RDDFORMAT_JSON = "json"

  require(parameters.isDefinedAt(TRINITY_SERVER), s"Option '$TRINITY_SERVER' is required.")
  require(parameters.isDefinedAt(TRINITY_CELLTYPE), s"Option '$TRINITY_CELLTYPE' is required.")

  val serverAddress = parameters(TRINITY_SERVER)
  val cellType = parameters(TRINITY_CELLTYPE)
  val batchSize = {
    val size = parameters.getOrElse(TRINITY_BATCHSIZE, s"$TRINITY_BATCHSIZE_DEFAULT").toInt
    require(size > 0, s"Invalid value `${size.toString}` for parameter " +
      s"$TRINITY_BATCHSIZE. The minimum value is 1.")
    size
  }
  val rddFormat = parameters.getOrElse(TRINITY_RDDFORMAT, TRINITY_RDDFORMAT_JSON)
}
