using System;
using Utility;

namespace Entity
{
    public class UserEntity
    {
        [DataField(new string[] { "create" })]
        public string Id { get; set; }

        [DataField(new string[] { "create", "update" })]
        public string Name { get; set; }

        [DataField(new string[] { "create", "update" })]
        public byte Sex { get; set; }

        [DataField(new string[] { "create", "update" })]
        public byte Age { get; set; }

        [DataField(new string[] { "create", "update" })]
        public string NativePlace { get; set; }

        [DataField(new string[] { "create", "update" })]
        public short Occupation { get; set; }

        [DataField(new string[] { "create", "update" })]
        public string Hobby { get; set; }

        [DataField(new string[] { "create" })]
        public string CreateTime { get; set; }

        [DataField(new string[] { "create", "update" })]
        public string Remark { get; set; }
    }

    public class UserSEntity
    {
        public string Name { get; set; }
        public byte Sex { get; set; }
        public string NativePlace { get; set; }
        public short Occupation { get; set; }
        public string Hobby { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
