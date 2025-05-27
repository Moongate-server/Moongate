#!/bin/bash

RELEASE="Debug"

dotnet publish -r osx-arm64 -o dist -p:PublishAot=true -c $RELEASE src/Moongate.Server \
  && ./dist/Moongate.Server "$@" \
  && rm -Rf moongate \
  && rm -Rf dist
