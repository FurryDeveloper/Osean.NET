using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Osean.NET {
    internal class OseanBinary {
        internal OseanBinary() {
            this._stream = new MemoryStream();
            this._reader = new BinaryReader(this._stream);
            this._writer = new BinaryWriter(this._stream);
        }

        internal OseanBinary(byte[] buffer) {
            this._stream = new MemoryStream(buffer);
            this._reader = new BinaryReader(this._stream);
            this._writer = new BinaryWriter(this._stream);
        }

        internal string ReadString() {
            return Encoding.ASCII.GetString(this._reader.ReadBytes(this._reader.ReadInt32()));
        }

        internal sbyte ReadI8() {
            return this._reader.ReadSByte();
        }

        internal short ReadI16() {
            return this._reader.ReadInt16();
        }

        internal int ReadI32() {
            return this._reader.ReadInt32();
        }

        internal long ReadI64() {
            return this._reader.ReadInt64();
        }

        internal byte ReadU8() {
            return this._reader.ReadByte();
        }

        internal ushort ReadU16() {
            return this._reader.ReadUInt16();
        }

        internal uint ReadU32() {
            return this._reader.ReadUInt32();
        }

        internal ulong ReadU64() {
            return this._reader.ReadUInt64();
        }

        internal float ReadR32() {
            return this._reader.ReadSingle();
        }

        internal Double ReadR64() {
            return this._reader.ReadDouble();
        }

        internal void WriteString(string value) {
            this._writer.Write(value.Length);
            this._writer.Write(Encoding.ASCII.GetBytes(value));
        }

        internal void WriteI8(sbyte value) {
            this._writer.Write(value);
        }

        internal void WriteI16(short value) {
            this._writer.Write(value);
        }

        internal void WriteI32(int value) {
            this._writer.Write(value);
        }

        internal void WriteI64(long value) {
            this._writer.Write(value);
        }

        internal void WriteU8(byte value) {
            this._writer.Write(value);
        }

        internal void WriteU16(ushort value) {
            this._writer.Write(value);
        }

        internal void WriteU32(uint value) {
            this._writer.Write(value);
        }

        internal void WriteU64(ulong value) {
            this._writer.Write(value);
        }

        internal void WriteR32(float value) {
            this._writer.Write(value);
        }

        internal void WriteR64(double value) {
            this._writer.Write(value);
        }

        internal long Length() {
            return this._stream.Length;
        }

        internal byte[] Buffer() {
            return this._stream.ToArray();
        }

        private readonly MemoryStream _stream;
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;
    }
}
