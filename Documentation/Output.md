## Using test case "Eslon AccessPoint"
```
(Main=(String="\0\a\b\t\n\v\f\n\"\\\xA0�",Boolean=False,Char="\0",Byte=255,SByte=127,Int16=32767,UInt16=65535,Int32=2147483647,UInt32=4294967295,Int64=9223372036854775807,UInt64=18446744073709551615,Single=3.40282347E+38,Double=1.7976931348623157E+308,Decimal="79228162514264337593543950335.0",DateTime="9999-12-31T23:59:59.999+01:00",DateTimeOffset="9999-12-31T23:59:59.999+00:00",TimeSpan="10675199.02:48:05.4775807",Guid="fe61c77e-07e9-438c-81f4-f659c373b115",Uri="http://www.microsoft.com/",ByteArray="00ff00"),Aux=(Nullable=?,NaN=NaN,Inf=Infinity,NInf=-Infinity),Enums=(Enum1=18446744073709551615,Enum2=32767,Enum3=0x0000000F),Collections=(Array={1,2,3},MultiArray={{1,2,3},{4,5,6},{7,8,9}},Dictionary={{"A",1},{"B",2},{"C",3}},List={1,2,3},Collection={1,2,3},Enumerable={1,2,3}))
```

## Using test case "Java AccessPoint"
```
{"Main":{"String":"\u0000\u0007\b\t\n\u000B\f\n\"\\\u00A0�","Boolean":false,"Char":"\u0000","Byte":255,"SByte":127,"Int16":32767,"UInt16":65535,"Int32":2147483647,"UInt32":4294967295,"Int64":9223372036854775807,"UInt64":18446744073709551615,"Single":3.40282347E+38,"Double":1.7976931348623157E+308,"Decimal":"79228162514264337593543950335.0","DateTime":"9999-12-31T23:59:59.999+01:00","DateTimeOffset":"9999-12-31T23:59:59.999+00:00","TimeSpan":"10675199.02:48:05.4775807","Guid":"d328e6e8-3534-4478-9ab8-118055b4a900","Uri":"http:\/\/www.microsoft.com\/","ByteArray":"AP8A"},"Aux":{"Nullable":null,"NaN":"NaN","Inf":"Infinity","NInf":"-Infinity"},"Enums":{"Enum1":18446744073709551615,"Enum2":32767,"Enum3":15},"Collections":{"Array":[1,2,3],"MultiArray":[[1,2,3],[4,5,6],[7,8,9]],"Dictionary":[["A",1],["B",2],["C",3]],"List":[1,2,3],"Collection":[1,2,3],"Enumerable":[1,2,3]}}
```

## Using test case "Eslon Batch"
```
^ Eslon Batch

Assembly: EslonTest (1.0.0.0)

@ Main
@ Aux
@ Enums
@ Collections
^

@ Main
  String = "\0\a\b\t\n\v\f\n\"\\\xA0�"
  Boolean = False
  Char = "\0"
  Byte = 255
  SByte = 127
  Int16 = 32767
  UInt16 = 65535
  Int32 = 2147483647
  UInt32 = 4294967295
  Int64 = 9223372036854775807
  UInt64 = 18446744073709551615
  Single = 3.40282347E+38
  Double = 1.7976931348623157E+308
  Decimal = "79228162514264337593543950335.0"
  DateTime = "9999-12-31T23:59:59.999+01:00"
  DateTimeOffset = "9999-12-31T23:59:59.999+00:00"
  TimeSpan = "10675199.02:48:05.4775807"
  Guid = "9232490c-2caa-481f-acb2-f69183a8ef4d"
  Uri = "http://www.microsoft.com/"
  ByteArray = "00ff00"

@ Aux
  Nullable = ?
  NaN = NaN
  Inf = Infinity
  NInf = -Infinity

@ Enums
  Enum1 = 18446744073709551615
  Enum2 = 32767
  Enum3 = 0x0000000F

@ Collections
  Array = {1,2,3}
  MultiArray = {{1,2,3},{4,5,6},{7,8,9}}
  Dictionary = {{"A",1},{"B",2},{"C",3}}
  List = {1,2,3}
  Collection = {1,2,3}
  Enumerable = {1,2,3}

# End
```