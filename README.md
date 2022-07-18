# jsonld-normalization-dotnet

The library implements JSON-LD normalization algorithm URDNA2015. 

It is based on [jsonld.js](https://github.com/digitalbazaar/jsonld.js) library, part of which has been ported to C#/.NET. Current version is based version 5.2.0 of jsonld.js. 

## Usage

Main entry point is `JsonLd` class, which exposes `Canonize` and `Normalize` functions, which transform input JSON-LD document to a canonized/normalized form. 
This can later be used e.g. for hashing in order to have consistent and reproducible hash values regardless of insignificant differences like fields ordering.
Both "Normalization" and "Canonization" names are used interchangeably in this context, so the two functions have the same effect.

### Example

```
await JsonLd.Normalize(File.ReadAllText(fileName));
```

## Contribute

### Run the tests

```
$ test .\code\JsonLd.Normalization.Test
```

## Contact

Contact us at [the Blockcerts community forum](http://community.blockcerts.org/).
