//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: proto/LoginMsg.proto
namespace proto.LoginMsg
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MsgRegister")]
  public partial class MsgRegister : global::ProtoBuf.IExtensible
  {
    public MsgRegister() {}
    
    private string _id = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string id
    {
      get { return _id; }
      set { _id = value; }
    }
    private string _pw = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"pw", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string pw
    {
      get { return _pw; }
      set { _pw = value; }
    }
    private int _result = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"result", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int result
    {
      get { return _result; }
      set { _result = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MsgLogin")]
  public partial class MsgLogin : global::ProtoBuf.IExtensible
  {
    public MsgLogin() {}
    
    private string _id = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string id
    {
      get { return _id; }
      set { _id = value; }
    }
    private string _pw = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"pw", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string pw
    {
      get { return _pw; }
      set { _pw = value; }
    }
    private int _result = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"result", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int result
    {
      get { return _result; }
      set { _result = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"MsgKick")]
  public partial class MsgKick : global::ProtoBuf.IExtensible
  {
    public MsgKick() {}
    
    private int _reson = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"reson", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int reson
    {
      get { return _reson; }
      set { _reson = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}