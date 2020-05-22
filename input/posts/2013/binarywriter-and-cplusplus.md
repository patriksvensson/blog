---
Title: System.IO.BinaryWriter and C++
Slug: binarywriter-and-cplusplus
Published: 2013-12-16
Tags:
- .NET
- C++
---

A while back I had to read string data written by .NET's [BinaryWriter](http://msdn.microsoft.com/en-us/library/system.io.binarywriter(v=vs.110).aspx) in C++. I was initially a little bit confused about how the data was written but after using Reflector it turned out that the write method prefixes the string with a [7-bit encoded integer](http://en.wikipedia.org/wiki/Variable-length_quantity). 

<!--excerpt-->

The encoded integer indicates the length of the written string, and is more commonly known as [UTF-7 encoding](http://en.wikipedia.org/wiki/UTF-7).

The short example below shows how the string length can be read from a standard input stream.

    int32_t BinaryReader::read7BitEncodedInteger(ifstream* stream)
    {
        char current;
        int32_t index = 0, result = 0;
        do
        {
            stream->read((char*)&current, sizeof(char));
            result |= (current & 127) << index;
            index += 7;
        }
        while((current & 128) != 0);
        return result;
    }