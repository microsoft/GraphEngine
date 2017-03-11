// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
package io.graphengine.spark

import org.apache.spark.sql.{DataFrame, DataFrameReader}

package object trinity {
  implicit class TrinityDataFrameReader(reader: DataFrameReader) {
    def trinity: String => DataFrame = reader.format("io.graphengine.spark.trinity").load
  }
}
