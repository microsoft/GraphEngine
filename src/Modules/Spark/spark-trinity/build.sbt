name := "spark-trinity"

version := "1.0"

organization := "io.graphengine"

scalaVersion := "2.11.8"

sparkVersion := "2.0.0"

sparkComponents := Seq("sql")

libraryDependencies ++= Seq(
  "org.scalaj" %% "scalaj-http" % "2.3.0",
  "org.json4s" %% "json4s-native" % "3.3.0"
)
