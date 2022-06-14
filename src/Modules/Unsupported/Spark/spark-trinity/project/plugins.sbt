resolvers += "bintray-spark-packages" at "https://dl.bintray.com/spark-packages/maven/"

resolvers += "JBoss" at "https://repository.jboss.org"

addSbtPlugin("org.spark-packages" % "sbt-spark-package" % "0.2.5")
