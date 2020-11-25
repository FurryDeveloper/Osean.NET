using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Osean.NET {
    internal class OseanPacket {
        internal OseanPacket(uint id, List<OseanPacketField> fields) {
            this.ID = id;
            this.Fields = fields;
        }

        internal uint ID { get; }
        internal List<OseanPacketField> Fields { get; }
    }
}