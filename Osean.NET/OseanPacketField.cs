using System.Reflection;

namespace Osean.NET {
    internal class OseanPacketField {
        internal OseanPacketField(FieldInfo field, OseanDataType dataType) {
            this.Field = field;
            this.DataType = dataType;
        }

        internal FieldInfo Field { get; }
        internal OseanDataType DataType { get; }
    }
}