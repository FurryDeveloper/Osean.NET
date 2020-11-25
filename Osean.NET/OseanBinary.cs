using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Osean.NET {
    internal class OseanBinary {
        public OseanBinary() {
            this._stream = new MemoryStream();
            this._reader = new BinaryReader(this._stream);
            this._writer = new BinaryWriter(this._stream);
        }

        public OseanBinary(byte[] buffer) {
            this._stream = new MemoryStream(buffer);
            this._reader = new BinaryReader(this._stream);
            this._writer = new BinaryWriter(this._stream);
        }

        public long Tell() {
            return this._stream.Position;
        }

        public void Seek(long pos) {
            this._stream.Position = pos;
        }

        public long Size() {
            return this._stream.Length;
        }

        public string ReadString() {
            return Encoding.ASCII.GetString(this._reader.ReadBytes((int) this._reader.ReadUInt32()));
        }

        public string ReadWString() {
            return Encoding.UTF8.GetString(this._reader.ReadBytes((int) this._reader.ReadUInt32()));
        }

        public sbyte ReadI8() {
            return this._reader.ReadSByte();
        }

        public short ReadI16() {
            return this._reader.ReadInt16();
        }

        public int ReadI32() {
            return this._reader.ReadInt32();
        }

        public long ReadI64() {
            return this._reader.ReadInt64();
        }

        public byte ReadU8() {
            return this._reader.ReadByte();
        }

        public ushort ReadU16() {
            return this._reader.ReadUInt16();
        }

        public uint ReadU32() {
            return this._reader.ReadUInt32();
        }

        public ulong ReadU64() {
            return this._reader.ReadUInt64();
        }

        public float ReadR32() {
            return this._reader.ReadSingle();
        }

        public Double ReadR64() {
            return this._reader.ReadDouble();
        }

        public void WriteString(string value) {
            this._writer.Write(value.Length);
            this._writer.Write(Encoding.ASCII.GetBytes(value));
        }

        public void WriteWString(string value) {
            this._writer.Write(value.Length);
            this._writer.Write(Encoding.Unicode.GetBytes(value));
        }

        public void WriteI8(sbyte value) {
            this._writer.Write(value);
        }

        public void WriteI16(short value) {
            this._writer.Write(value);
        }

        public void WriteI32(int value) {
            this._writer.Write(value);
        }

        public void WriteI64(long value) {
            this._writer.Write(value);
        }

        public void WriteU8(byte value) {
            this._writer.Write(value);
        }

        public void WriteU16(ushort value) {
            this._writer.Write(value);
        }

        public void WriteU32(uint value) {
            this._writer.Write(value);
        }

        public void WriteU64(ulong value) {
            this._writer.Write(value);
        }

        public void WriteR32(float value) {
            this._writer.Write(value);
        }

        public void WriteR64(double value) {
            this._writer.Write(value);
        }

        public long Length() {
            return this._stream.Length;
        }

        public byte[] Buffer() {
            return this._stream.ToArray();
        }

        private readonly MemoryStream _stream;
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;
    }
}