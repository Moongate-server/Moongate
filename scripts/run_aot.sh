#!/bin/bash

dotnet publish -r osx-arm64 -o dist -p:PublishAot=true -c Release src/Moongate.Server \
  && ./dist/Moongate.Server "$@" \
  && rm -Rf moongate \
  && rm -Rf dist
