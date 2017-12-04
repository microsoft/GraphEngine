// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
package io.graphengine.spark.trinity

class TrinityOptions(@transient private val parameters: Map[String, String]) extends Serializable{

  val TRINITY_PATH = "path"
  val TRINITY_SERVERADDR = "serverAddress"
  val TRINITY_CELLTYPE = "cellType"
  val TRINITY_BATCHSIZE = "batchSize"
  val TRINITY_RDDFORMAT = "rddFormat"

  val TRINITY_SERVERADDR_SCHEME_DEFAULT = "http"
  val TRINITY_SERVERADDR_HOST_DEFAULT = "localhost"
  val TRINITY_BATCHSIZE_DEFAULT = 1000
  val TRINITY_RDDFORMAT_JSON = "json"

  require(parameters.isDefinedAt(TRINITY_PATH), s"Option '$TRINITY_PATH' is required.")

  val url = {
    val path = parameters(TRINITY_PATH)
    val u = new java.net.URI(path)
    val scheme = Option(u.getScheme).getOrElse(TRINITY_SERVERADDR_SCHEME_DEFAULT)
    val host = Option(u.getHost).getOrElse(TRINITY_SERVERADDR_HOST_DEFAULT)
    val pathInUri = Option(u.getPath).getOrElse("").trim

    val serverAddress = if (u.getPort == -1) s"$scheme://$host" else s"$scheme://$host:$u.getPort"
    val cellType = if (pathInUri.startsWith("/")) pathInUri.substring(1) else pathInUri
    require(cellType.length > 0, s"Missing $TRINITY_CELLTYPE in $TRINITY_PATH: `$path`")
    Map(TRINITY_SERVERADDR->serverAddress, TRINITY_CELLTYPE->cellType)
  }

  val serverAddress = url("serverAddress")
  val cellType = url("cellType")
  val batchSize = {
    val size = parameters.getOrElse(TRINITY_BATCHSIZE, s"$TRINITY_BATCHSIZE_DEFAULT").toInt
    require(size > 0, s"Invalid value `${size.toString}` for parameter " +
      s"$TRINITY_BATCHSIZE. The minimum value is 1.")
    size
  }
  val rddFormat = parameters.getOrElse(TRINITY_RDDFORMAT, TRINITY_RDDFORMAT_JSON)
}
