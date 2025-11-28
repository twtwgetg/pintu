#!/bin/bash

WORKSPACE=..
LUBAN_DLL=$WORKSPACE/Tools/Luban/Luban.dll
CONF_ROOT=.
OUTPUT_DATA_DIR=../../client/Assets/data
OUTPUT_CODE_DIR=../../client/Assets/Scripts/Config

# 创建输出目录
mkdir -p $OUTPUT_DATA_DIR
mkdir -p $OUTPUT_CODE_DIR

dotnet $LUBAN_DLL \
    -t all \
    -c cs-simple-json \
    -d json \
    --conf $CONF_ROOT/luban.conf \
    -x outputDataDir=$OUTPUT_DATA_DIR \
    -x outputCodeDir=$OUTPUT_CODE_DIR