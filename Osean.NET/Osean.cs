using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Osean.NET {
    public class Osean {
        public void RegisterPacket<T>(uint id) {
            if (this._packets.ContainsKey(id)) {
                throw new InvalidOperationException($"packet with ID of ({id}) is already registered!");
            }

            var fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var packetFields = new List<OseanPacketField>();

            foreach (var field in fields) {
                var type = this.GetTypeDataType(field.FieldType);

                packetFields.Add(new OseanPacketField(field, type));

                if (type == OseanDataType.Array) {
                    packetFields.Add(new OseanPacketField(field, this.GetTypeDataType(field.FieldType.GetElementType())));
                }
            }

            this._packets.Add(id, new OseanPacket(id, packetFields));
        }

        public bool EncodePacket<T>(uint id, T packet, out byte[] buffer) {
            buffer = null;

            if (!this._packets.ContainsKey(id))
                return false;

            var binary = new OseanBinary();

            try {
                var fields = this._packets[id].Fields;

                for (var i = 0; i < fields.Count; ++i) {
                    var field = fields[i];

                    var fieldType = fields[i].DataType;
                    var nextFieldType = i + 1 < fields.Count
                        ? fields[i + 1].DataType
                        : OseanDataType.None;

                    this.WriteValue(binary, fieldType, nextFieldType, field.Field.GetValue(packet));

                    if (fieldType == OseanDataType.Array) {
                        ++i;
                    }
                }
            }
            catch {
                return false;
            }

            buffer = binary.Buffer();

            return true;
        }

        public bool DecodePacket<T>(uint id, out T packet, byte[] buffer) {
            var binary = new OseanBinary(buffer);

            try {
                packet = Activator.CreateInstance<T>();

                var fields = this._packets[id].Fields;

                for (var i = 0; i < fields.Count; ++i) {
                    var field = fields[i];

                    var fieldType = fields[i].DataType;
                    var nextFieldType = i + 1 < fields.Count
                        ? fields[i + 1].DataType
                        : OseanDataType.None;

                    field.Field.SetValue(packet, this.ReadValue(binary, fieldType, nextFieldType));

                    if (fieldType == OseanDataType.Array) {
                        ++i;
                    }
                }
            }
            catch {
                packet = default(T);
                return false;
            }

            return true;
        }

        private object ReadValue(OseanBinary binary, OseanDataType type, OseanDataType arrayType) {
            return type switch {
                OseanDataType.I8 => binary.ReadI8(),
                OseanDataType.I16 => binary.ReadI16(),
                OseanDataType.I32 => binary.ReadI32(),
                OseanDataType.I64 => binary.ReadI64(),

                OseanDataType.U8 => binary.ReadU8(),
                OseanDataType.U16 => binary.ReadU16(),
                OseanDataType.U32 => binary.ReadU32(),
                OseanDataType.U64 => binary.ReadU64(),

                OseanDataType.R32 => binary.ReadR32(),
                OseanDataType.R64 => binary.ReadR64(),

                OseanDataType.Boolean => binary.ReadU8() != 0,
                OseanDataType.String => binary.ReadString(),
                OseanDataType.WString => binary.ReadWString(),

                OseanDataType.Array => ((Func<Array>) (() => {
                    var array = Array.CreateInstance(this.TypeFromDataType(arrayType), binary.ReadI32());

                    for (var i = 0; i < array.Length; ++i) {
                        array.SetValue(this.ReadValue(binary, arrayType, OseanDataType.None), i);
                    }

                    return array;
                }))(),

                _ => throw new InvalidOperationException("cannot read invalid type code!")
            };
        }

        private void WriteValue(OseanBinary binary, OseanDataType type, OseanDataType arrayType, object value) {
            switch (type) {
                case OseanDataType.I8:
                    binary.WriteI8((sbyte) value);
                    break;

                case OseanDataType.I16:
                    binary.WriteI16((short) value);
                    break;

                case OseanDataType.I32:
                    binary.WriteI32((int) value);
                    break;

                case OseanDataType.I64:
                    binary.WriteI64((long) value);
                    break;

                case OseanDataType.U8:
                    binary.WriteU8((byte) value);
                    break;

                case OseanDataType.U16:
                    binary.WriteU16((ushort) value);
                    break;

                case OseanDataType.U32:
                    binary.WriteU32((uint) value);
                    break;

                case OseanDataType.U64:
                    binary.WriteU64((ulong) value);
                    break;

                case OseanDataType.R32:
                    binary.WriteR32((float) value);
                    break;

                case OseanDataType.R64:
                    binary.WriteR64((double) value);
                    break;

                case OseanDataType.Boolean:
                    binary.WriteU8((byte) ((bool) value ? 1 : 0));
                    break;

                case OseanDataType.String:
                    binary.WriteString((string) value);
                    break;

                case OseanDataType.WString:
                    binary.WriteWString((string) value);
                    break;

                case OseanDataType.Array: {
                    var array = (Array) value;

                    binary.WriteI32(array.Length);

                    for (var i = 0; i < array.Length; ++i) {
                        this.WriteValue(binary, arrayType, OseanDataType.None, array.GetValue(i));
                    }

                    break;
                }

                default:
                    throw new InvalidOperationException("cannot write invalid type code!");
            }
        }

        private OseanDataType GetTypeDataType(Type type) {
            return Type.GetTypeCode(type) switch {
                TypeCode.SByte => OseanDataType.I8,
                TypeCode.Int16 => OseanDataType.I16,
                TypeCode.Int32 => OseanDataType.I32,
                TypeCode.Int64 => OseanDataType.I64,

                TypeCode.Byte => OseanDataType.U8,
                TypeCode.UInt16 => OseanDataType.U16,
                TypeCode.UInt32 => OseanDataType.U32,
                TypeCode.UInt64 => OseanDataType.U64,

                TypeCode.Single => OseanDataType.R32,
                TypeCode.Double => OseanDataType.R64,

                TypeCode.Boolean => OseanDataType.Boolean,
                TypeCode.String => OseanDataType.String,

                _ => type.IsArray
                    ? OseanDataType.Array
                    : throw new InvalidOperationException("unsupported data type")
            };
        }

        private Type TypeFromDataType(OseanDataType dataType) {
            return dataType switch {
                OseanDataType.I8 => typeof(sbyte),
                OseanDataType.I16 => typeof(short),
                OseanDataType.I32 => typeof(int),
                OseanDataType.I64 => typeof(long),

                OseanDataType.U8 => typeof(byte),
                OseanDataType.U16 => typeof(ushort),
                OseanDataType.U32 => typeof(uint),
                OseanDataType.U64 => typeof(long),

                OseanDataType.R32 => typeof(float),
                OseanDataType.R64 => typeof(double),

                OseanDataType.String => typeof(string),
                OseanDataType.WString => typeof(string),

                _ => throw new InvalidOperationException("invalid typeof(T)")
            };
        }

        private Dictionary<uint, OseanPacket> _packets = new Dictionary<uint, OseanPacket>();
    }
}