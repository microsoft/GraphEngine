# Graph Engine DataImporter

DataImporter is a cross-platform module of Microsoft [Graph Engine](https://www.graphengine.io/) for [data importing](https://www.graphengine.io/docs/manual/DataImport.html). It supports to import data files in types of CSV, TSV, JSON and RDF, saving input data to Graph Engine storages.

## Usage
The input files include the data file(s) and a [TSL
file](https://www.graphengine.io/docs/manual/TSL/tsl-basics.html) which
describes the schema of the data. The output is Graph Engine data storage.

The data files in JSON (.json), CSV (.csv), TSV (.tsv), and N-Triples
(.ntriples) for are supported. Note that the filename extensions of the data
files are used to recognize the type of the data.

In the `GraphEngine.DataImporter` folder:

- Build the required NuGet packages by running `tools/build.ps1` in the root directory of this repository
- cd `src/Modules/DataImporter/GraphEngine.DataImporter`
- Run `dotnet restore`
- Run `dotnet build --configuration Release`
- Run `dotnet run --framework net6.0 --property:Configuration=Release --no-build -- --help`

### Command Line Option

|Option | Description|
|:-----|:------------|
|-t, --tsl|Specifies the TSL file for data importing.|
|-d, --dir|Import all .json and .txt files from directory.|
|-o, --output|Specifies data import output directory for importing tasks, and specifies the output TSL file name for TSL generation tasks.|
|-g, --generate_tsl|Generates TSL.|
|-s, --sorted|Specifies that the data is already sorted/grouped by entities.|
|--delimiter|Specifies the delimiter of CSV or TSV file.|
|-f, --fileFormat|Specifies the file format.|
|--notrim|Specifies that the data fields in CSV/TSV files are not trimmed.|
|-a, --tsl_assembly|Specifies the TSL assembly for data importing.|
|--help|Display this help screen.|

```shell
dotnet run --framework net6.0 --property:Configuration=Release --no-build -- [-t tsl] [-d directory] [-o output_dir] [--delimiter delimiter] [-f file_format] [--notrim] [-a tsl_assembly|-g] [explicit files]
```
