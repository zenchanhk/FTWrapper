// Generated by ProtoGen, Version=2.4.1.555, Culture=neutral, PublicKeyToken=17b3b1f090c3ea48.  DO NOT EDIT!
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.ProtocolBuffers;
using pbc = global::Google.ProtocolBuffers.Collections;
using pbd = global::Google.ProtocolBuffers.Descriptors;
using scg = global::System.Collections.Generic;
namespace Futu.OpenApi.Pb {
  
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  public static partial class QotUpdateRT {
  
    #region Extension registration
    public static void RegisterAllExtensions(pb::ExtensionRegistry registry) {
    }
    #endregion
    #region Static variables
    private static pbd::MessageDescriptor internal__static_Qot_UpdateRT_S2C__Descriptor;
    private static pb::FieldAccess.FieldAccessorTable<global::Futu.OpenApi.Pb.QotUpdateRT.S2C, global::Futu.OpenApi.Pb.QotUpdateRT.S2C.Builder> internal__static_Qot_UpdateRT_S2C__FieldAccessorTable;
    private static pbd::MessageDescriptor internal__static_Qot_UpdateRT_Response__Descriptor;
    private static pb::FieldAccess.FieldAccessorTable<global::Futu.OpenApi.Pb.QotUpdateRT.Response, global::Futu.OpenApi.Pb.QotUpdateRT.Response.Builder> internal__static_Qot_UpdateRT_Response__FieldAccessorTable;
    #endregion
    #region Descriptor
    public static pbd::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbd::FileDescriptor descriptor;
    
    static QotUpdateRT() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChJRb3RfVXBkYXRlUlQucHJvdG8SDFFvdF9VcGRhdGVSVBoMQ29tbW9uLnBy", 
            "b3RvGhBRb3RfQ29tbW9uLnByb3RvImYKA1MyQxIwCghzZWN1cml0eRgBIAIo", 
            "CzIULlFvdF9Db21tb24uU2VjdXJpdHlSCHNlY3VyaXR5Ei0KBnJ0TGlzdBgC", 
            "IAMoCzIVLlFvdF9Db21tb24uVGltZVNoYXJlUgZydExpc3QigQEKCFJlc3Bv", 
            "bnNlEh4KB3JldFR5cGUYASACKAU6BC00MDBSB3JldFR5cGUSFgoGcmV0TXNn", 
            "GAIgASgJUgZyZXRNc2cSGAoHZXJyQ29kZRgDIAEoBVIHZXJyQ29kZRIjCgNz", 
            "MmMYBCABKAsyES5Rb3RfVXBkYXRlUlQuUzJDUgNzMmNCFQoTY29tLmZ1dHUu", 
          "b3BlbmFwaS5wYg=="));
      pbd::FileDescriptor.InternalDescriptorAssigner assigner = delegate(pbd::FileDescriptor root) {
        descriptor = root;
        internal__static_Qot_UpdateRT_S2C__Descriptor = Descriptor.MessageTypes[0];
        internal__static_Qot_UpdateRT_S2C__FieldAccessorTable = 
            new pb::FieldAccess.FieldAccessorTable<global::Futu.OpenApi.Pb.QotUpdateRT.S2C, global::Futu.OpenApi.Pb.QotUpdateRT.S2C.Builder>(internal__static_Qot_UpdateRT_S2C__Descriptor,
                new string[] { "Security", "RtList", });
        internal__static_Qot_UpdateRT_Response__Descriptor = Descriptor.MessageTypes[1];
        internal__static_Qot_UpdateRT_Response__FieldAccessorTable = 
            new pb::FieldAccess.FieldAccessorTable<global::Futu.OpenApi.Pb.QotUpdateRT.Response, global::Futu.OpenApi.Pb.QotUpdateRT.Response.Builder>(internal__static_Qot_UpdateRT_Response__Descriptor,
                new string[] { "RetType", "RetMsg", "ErrCode", "S2C", });
        pb::ExtensionRegistry registry = pb::ExtensionRegistry.CreateInstance();
        RegisterAllExtensions(registry);
        global::Futu.OpenApi.Pb.Common.RegisterAllExtensions(registry);
        global::Futu.OpenApi.Pb.QotCommon.RegisterAllExtensions(registry);
        return registry;
      };
      pbd::FileDescriptor.InternalBuildGeneratedFileFrom(descriptorData,
          new pbd::FileDescriptor[] {
          global::Futu.OpenApi.Pb.Common.Descriptor, 
          global::Futu.OpenApi.Pb.QotCommon.Descriptor, 
          }, assigner);
    }
    #endregion
    
    #region Messages
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public sealed partial class S2C : pb::GeneratedMessage<S2C, S2C.Builder> {
      private S2C() { }
      private static readonly S2C defaultInstance = new S2C().MakeReadOnly();
      private static readonly string[] _s2CFieldNames = new string[] { "rtList", "security" };
      private static readonly uint[] _s2CFieldTags = new uint[] { 18, 10 };
      public static S2C DefaultInstance {
        get { return defaultInstance; }
      }
      
      public override S2C DefaultInstanceForType {
        get { return DefaultInstance; }
      }
      
      protected override S2C ThisMessage {
        get { return this; }
      }
      
      public static pbd::MessageDescriptor Descriptor {
        get { return global::Futu.OpenApi.Pb.QotUpdateRT.internal__static_Qot_UpdateRT_S2C__Descriptor; }
      }
      
      protected override pb::FieldAccess.FieldAccessorTable<S2C, S2C.Builder> InternalFieldAccessors {
        get { return global::Futu.OpenApi.Pb.QotUpdateRT.internal__static_Qot_UpdateRT_S2C__FieldAccessorTable; }
      }
      
      public const int SecurityFieldNumber = 1;
      private bool hasSecurity;
      private global::Futu.OpenApi.Pb.QotCommon.Security security_;
      public bool HasSecurity {
        get { return hasSecurity; }
      }
      public global::Futu.OpenApi.Pb.QotCommon.Security Security {
        get { return security_ ?? global::Futu.OpenApi.Pb.QotCommon.Security.DefaultInstance; }
      }
      
      public const int RtListFieldNumber = 2;
      private pbc::PopsicleList<global::Futu.OpenApi.Pb.QotCommon.TimeShare> rtList_ = new pbc::PopsicleList<global::Futu.OpenApi.Pb.QotCommon.TimeShare>();
      public scg::IList<global::Futu.OpenApi.Pb.QotCommon.TimeShare> RtListList {
        get { return rtList_; }
      }
      public int RtListCount {
        get { return rtList_.Count; }
      }
      public global::Futu.OpenApi.Pb.QotCommon.TimeShare GetRtList(int index) {
        return rtList_[index];
      }
      
      public override bool IsInitialized {
        get {
          if (!hasSecurity) return false;
          if (!Security.IsInitialized) return false;
          foreach (global::Futu.OpenApi.Pb.QotCommon.TimeShare element in RtListList) {
            if (!element.IsInitialized) return false;
          }
          return true;
        }
      }
      
      public override void WriteTo(pb::ICodedOutputStream output) {
        CalcSerializedSize();
        string[] field_names = _s2CFieldNames;
        if (hasSecurity) {
          output.WriteMessage(1, field_names[1], Security);
        }
        if (rtList_.Count > 0) {
          output.WriteMessageArray(2, field_names[0], rtList_);
        }
        UnknownFields.WriteTo(output);
      }
      
      private int memoizedSerializedSize = -1;
      public override int SerializedSize {
        get {
          int size = memoizedSerializedSize;
          if (size != -1) return size;
          return CalcSerializedSize();
        }
      }
      
      private int CalcSerializedSize() {
        int size = memoizedSerializedSize;
        if (size != -1) return size;
        
        size = 0;
        if (hasSecurity) {
          size += pb::CodedOutputStream.ComputeMessageSize(1, Security);
        }
        foreach (global::Futu.OpenApi.Pb.QotCommon.TimeShare element in RtListList) {
          size += pb::CodedOutputStream.ComputeMessageSize(2, element);
        }
        size += UnknownFields.SerializedSize;
        memoizedSerializedSize = size;
        return size;
      }
      public static S2C ParseFrom(pb::ByteString data) {
        return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
      }
      public static S2C ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
        return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
      }
      public static S2C ParseFrom(byte[] data) {
        return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
      }
      public static S2C ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
        return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
      }
      public static S2C ParseFrom(global::System.IO.Stream input) {
        return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
      }
      public static S2C ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
        return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
      }
      public static S2C ParseDelimitedFrom(global::System.IO.Stream input) {
        return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
      }
      public static S2C ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
        return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
      }
      public static S2C ParseFrom(pb::ICodedInputStream input) {
        return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
      }
      public static S2C ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
        return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
      }
      private S2C MakeReadOnly() {
        rtList_.MakeReadOnly();
        return this;
      }
      
      public static Builder CreateBuilder() { return new Builder(); }
      public override Builder ToBuilder() { return CreateBuilder(this); }
      public override Builder CreateBuilderForType() { return new Builder(); }
      public static Builder CreateBuilder(S2C prototype) {
        return new Builder(prototype);
      }
      
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
      public sealed partial class Builder : pb::GeneratedBuilder<S2C, Builder> {
        protected override Builder ThisBuilder {
          get { return this; }
        }
        public Builder() {
          result = DefaultInstance;
          resultIsReadOnly = true;
        }
        internal Builder(S2C cloneFrom) {
          result = cloneFrom;
          resultIsReadOnly = true;
        }
        
        private bool resultIsReadOnly;
        private S2C result;
        
        private S2C PrepareBuilder() {
          if (resultIsReadOnly) {
            S2C original = result;
            result = new S2C();
            resultIsReadOnly = false;
            MergeFrom(original);
          }
          return result;
        }
        
        public override bool IsInitialized {
          get { return result.IsInitialized; }
        }
        
        protected override S2C MessageBeingBuilt {
          get { return PrepareBuilder(); }
        }
        
        public override Builder Clear() {
          result = DefaultInstance;
          resultIsReadOnly = true;
          return this;
        }
        
        public override Builder Clone() {
          if (resultIsReadOnly) {
            return new Builder(result);
          } else {
            return new Builder().MergeFrom(result);
          }
        }
        
        public override pbd::MessageDescriptor DescriptorForType {
          get { return global::Futu.OpenApi.Pb.QotUpdateRT.S2C.Descriptor; }
        }
        
        public override S2C DefaultInstanceForType {
          get { return global::Futu.OpenApi.Pb.QotUpdateRT.S2C.DefaultInstance; }
        }
        
        public override S2C BuildPartial() {
          if (resultIsReadOnly) {
            return result;
          }
          resultIsReadOnly = true;
          return result.MakeReadOnly();
        }
        
        public override Builder MergeFrom(pb::IMessage other) {
          if (other is S2C) {
            return MergeFrom((S2C) other);
          } else {
            base.MergeFrom(other);
            return this;
          }
        }
        
        public override Builder MergeFrom(S2C other) {
          if (other == global::Futu.OpenApi.Pb.QotUpdateRT.S2C.DefaultInstance) return this;
          PrepareBuilder();
          if (other.HasSecurity) {
            MergeSecurity(other.Security);
          }
          if (other.rtList_.Count != 0) {
            result.rtList_.Add(other.rtList_);
          }
          this.MergeUnknownFields(other.UnknownFields);
          return this;
        }
        
        public override Builder MergeFrom(pb::ICodedInputStream input) {
          return MergeFrom(input, pb::ExtensionRegistry.Empty);
        }
        
        public override Builder MergeFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
          PrepareBuilder();
          pb::UnknownFieldSet.Builder unknownFields = null;
          uint tag;
          string field_name;
          while (input.ReadTag(out tag, out field_name)) {
            if(tag == 0 && field_name != null) {
              int field_ordinal = global::System.Array.BinarySearch(_s2CFieldNames, field_name, global::System.StringComparer.Ordinal);
              if(field_ordinal >= 0)
                tag = _s2CFieldTags[field_ordinal];
              else {
                if (unknownFields == null) {
                  unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
                }
                ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
                continue;
              }
            }
            switch (tag) {
              case 0: {
                throw pb::InvalidProtocolBufferException.InvalidTag();
              }
              default: {
                if (pb::WireFormat.IsEndGroupTag(tag)) {
                  if (unknownFields != null) {
                    this.UnknownFields = unknownFields.Build();
                  }
                  return this;
                }
                if (unknownFields == null) {
                  unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
                }
                ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
                break;
              }
              case 10: {
                global::Futu.OpenApi.Pb.QotCommon.Security.Builder subBuilder = global::Futu.OpenApi.Pb.QotCommon.Security.CreateBuilder();
                if (result.hasSecurity) {
                  subBuilder.MergeFrom(Security);
                }
                input.ReadMessage(subBuilder, extensionRegistry);
                Security = subBuilder.BuildPartial();
                break;
              }
              case 18: {
                input.ReadMessageArray(tag, field_name, result.rtList_, global::Futu.OpenApi.Pb.QotCommon.TimeShare.DefaultInstance, extensionRegistry);
                break;
              }
            }
          }
          
          if (unknownFields != null) {
            this.UnknownFields = unknownFields.Build();
          }
          return this;
        }
        
        
        public bool HasSecurity {
         get { return result.hasSecurity; }
        }
        public global::Futu.OpenApi.Pb.QotCommon.Security Security {
          get { return result.Security; }
          set { SetSecurity(value); }
        }
        public Builder SetSecurity(global::Futu.OpenApi.Pb.QotCommon.Security value) {
          pb::ThrowHelper.ThrowIfNull(value, "value");
          PrepareBuilder();
          result.hasSecurity = true;
          result.security_ = value;
          return this;
        }
        public Builder SetSecurity(global::Futu.OpenApi.Pb.QotCommon.Security.Builder builderForValue) {
          pb::ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
          PrepareBuilder();
          result.hasSecurity = true;
          result.security_ = builderForValue.Build();
          return this;
        }
        public Builder MergeSecurity(global::Futu.OpenApi.Pb.QotCommon.Security value) {
          pb::ThrowHelper.ThrowIfNull(value, "value");
          PrepareBuilder();
          if (result.hasSecurity &&
              result.security_ != global::Futu.OpenApi.Pb.QotCommon.Security.DefaultInstance) {
              result.security_ = global::Futu.OpenApi.Pb.QotCommon.Security.CreateBuilder(result.security_).MergeFrom(value).BuildPartial();
          } else {
            result.security_ = value;
          }
          result.hasSecurity = true;
          return this;
        }
        public Builder ClearSecurity() {
          PrepareBuilder();
          result.hasSecurity = false;
          result.security_ = null;
          return this;
        }
        
        public pbc::IPopsicleList<global::Futu.OpenApi.Pb.QotCommon.TimeShare> RtListList {
          get { return PrepareBuilder().rtList_; }
        }
        public int RtListCount {
          get { return result.RtListCount; }
        }
        public global::Futu.OpenApi.Pb.QotCommon.TimeShare GetRtList(int index) {
          return result.GetRtList(index);
        }
        public Builder SetRtList(int index, global::Futu.OpenApi.Pb.QotCommon.TimeShare value) {
          pb::ThrowHelper.ThrowIfNull(value, "value");
          PrepareBuilder();
          result.rtList_[index] = value;
          return this;
        }
        public Builder SetRtList(int index, global::Futu.OpenApi.Pb.QotCommon.TimeShare.Builder builderForValue) {
          pb::ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
          PrepareBuilder();
          result.rtList_[index] = builderForValue.Build();
          return this;
        }
        public Builder AddRtList(global::Futu.OpenApi.Pb.QotCommon.TimeShare value) {
          pb::ThrowHelper.ThrowIfNull(value, "value");
          PrepareBuilder();
          result.rtList_.Add(value);
          return this;
        }
        public Builder AddRtList(global::Futu.OpenApi.Pb.QotCommon.TimeShare.Builder builderForValue) {
          pb::ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
          PrepareBuilder();
          result.rtList_.Add(builderForValue.Build());
          return this;
        }
        public Builder AddRangeRtList(scg::IEnumerable<global::Futu.OpenApi.Pb.QotCommon.TimeShare> values) {
          PrepareBuilder();
          result.rtList_.Add(values);
          return this;
        }
        public Builder ClearRtList() {
          PrepareBuilder();
          result.rtList_.Clear();
          return this;
        }
      }
      static S2C() {
        object.ReferenceEquals(global::Futu.OpenApi.Pb.QotUpdateRT.Descriptor, null);
      }
    }
    
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public sealed partial class Response : pb::GeneratedMessage<Response, Response.Builder> {
      private Response() { }
      private static readonly Response defaultInstance = new Response().MakeReadOnly();
      private static readonly string[] _responseFieldNames = new string[] { "errCode", "retMsg", "retType", "s2c" };
      private static readonly uint[] _responseFieldTags = new uint[] { 24, 18, 8, 34 };
      public static Response DefaultInstance {
        get { return defaultInstance; }
      }
      
      public override Response DefaultInstanceForType {
        get { return DefaultInstance; }
      }
      
      protected override Response ThisMessage {
        get { return this; }
      }
      
      public static pbd::MessageDescriptor Descriptor {
        get { return global::Futu.OpenApi.Pb.QotUpdateRT.internal__static_Qot_UpdateRT_Response__Descriptor; }
      }
      
      protected override pb::FieldAccess.FieldAccessorTable<Response, Response.Builder> InternalFieldAccessors {
        get { return global::Futu.OpenApi.Pb.QotUpdateRT.internal__static_Qot_UpdateRT_Response__FieldAccessorTable; }
      }
      
      public const int RetTypeFieldNumber = 1;
      private bool hasRetType;
      private int retType_ = -400;
      public bool HasRetType {
        get { return hasRetType; }
      }
      public int RetType {
        get { return retType_; }
      }
      
      public const int RetMsgFieldNumber = 2;
      private bool hasRetMsg;
      private string retMsg_ = "";
      public bool HasRetMsg {
        get { return hasRetMsg; }
      }
      public string RetMsg {
        get { return retMsg_; }
      }
      
      public const int ErrCodeFieldNumber = 3;
      private bool hasErrCode;
      private int errCode_;
      public bool HasErrCode {
        get { return hasErrCode; }
      }
      public int ErrCode {
        get { return errCode_; }
      }
      
      public const int S2CFieldNumber = 4;
      private bool hasS2C;
      private global::Futu.OpenApi.Pb.QotUpdateRT.S2C s2C_;
      public bool HasS2C {
        get { return hasS2C; }
      }
      public global::Futu.OpenApi.Pb.QotUpdateRT.S2C S2C {
        get { return s2C_ ?? global::Futu.OpenApi.Pb.QotUpdateRT.S2C.DefaultInstance; }
      }
      
      public override bool IsInitialized {
        get {
          if (!hasRetType) return false;
          if (HasS2C) {
            if (!S2C.IsInitialized) return false;
          }
          return true;
        }
      }
      
      public override void WriteTo(pb::ICodedOutputStream output) {
        CalcSerializedSize();
        string[] field_names = _responseFieldNames;
        if (hasRetType) {
          output.WriteInt32(1, field_names[2], RetType);
        }
        if (hasRetMsg) {
          output.WriteString(2, field_names[1], RetMsg);
        }
        if (hasErrCode) {
          output.WriteInt32(3, field_names[0], ErrCode);
        }
        if (hasS2C) {
          output.WriteMessage(4, field_names[3], S2C);
        }
        UnknownFields.WriteTo(output);
      }
      
      private int memoizedSerializedSize = -1;
      public override int SerializedSize {
        get {
          int size = memoizedSerializedSize;
          if (size != -1) return size;
          return CalcSerializedSize();
        }
      }
      
      private int CalcSerializedSize() {
        int size = memoizedSerializedSize;
        if (size != -1) return size;
        
        size = 0;
        if (hasRetType) {
          size += pb::CodedOutputStream.ComputeInt32Size(1, RetType);
        }
        if (hasRetMsg) {
          size += pb::CodedOutputStream.ComputeStringSize(2, RetMsg);
        }
        if (hasErrCode) {
          size += pb::CodedOutputStream.ComputeInt32Size(3, ErrCode);
        }
        if (hasS2C) {
          size += pb::CodedOutputStream.ComputeMessageSize(4, S2C);
        }
        size += UnknownFields.SerializedSize;
        memoizedSerializedSize = size;
        return size;
      }
      public static Response ParseFrom(pb::ByteString data) {
        return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
      }
      public static Response ParseFrom(pb::ByteString data, pb::ExtensionRegistry extensionRegistry) {
        return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
      }
      public static Response ParseFrom(byte[] data) {
        return ((Builder) CreateBuilder().MergeFrom(data)).BuildParsed();
      }
      public static Response ParseFrom(byte[] data, pb::ExtensionRegistry extensionRegistry) {
        return ((Builder) CreateBuilder().MergeFrom(data, extensionRegistry)).BuildParsed();
      }
      public static Response ParseFrom(global::System.IO.Stream input) {
        return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
      }
      public static Response ParseFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
        return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
      }
      public static Response ParseDelimitedFrom(global::System.IO.Stream input) {
        return CreateBuilder().MergeDelimitedFrom(input).BuildParsed();
      }
      public static Response ParseDelimitedFrom(global::System.IO.Stream input, pb::ExtensionRegistry extensionRegistry) {
        return CreateBuilder().MergeDelimitedFrom(input, extensionRegistry).BuildParsed();
      }
      public static Response ParseFrom(pb::ICodedInputStream input) {
        return ((Builder) CreateBuilder().MergeFrom(input)).BuildParsed();
      }
      public static Response ParseFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
        return ((Builder) CreateBuilder().MergeFrom(input, extensionRegistry)).BuildParsed();
      }
      private Response MakeReadOnly() {
        return this;
      }
      
      public static Builder CreateBuilder() { return new Builder(); }
      public override Builder ToBuilder() { return CreateBuilder(this); }
      public override Builder CreateBuilderForType() { return new Builder(); }
      public static Builder CreateBuilder(Response prototype) {
        return new Builder(prototype);
      }
      
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
      public sealed partial class Builder : pb::GeneratedBuilder<Response, Builder> {
        protected override Builder ThisBuilder {
          get { return this; }
        }
        public Builder() {
          result = DefaultInstance;
          resultIsReadOnly = true;
        }
        internal Builder(Response cloneFrom) {
          result = cloneFrom;
          resultIsReadOnly = true;
        }
        
        private bool resultIsReadOnly;
        private Response result;
        
        private Response PrepareBuilder() {
          if (resultIsReadOnly) {
            Response original = result;
            result = new Response();
            resultIsReadOnly = false;
            MergeFrom(original);
          }
          return result;
        }
        
        public override bool IsInitialized {
          get { return result.IsInitialized; }
        }
        
        protected override Response MessageBeingBuilt {
          get { return PrepareBuilder(); }
        }
        
        public override Builder Clear() {
          result = DefaultInstance;
          resultIsReadOnly = true;
          return this;
        }
        
        public override Builder Clone() {
          if (resultIsReadOnly) {
            return new Builder(result);
          } else {
            return new Builder().MergeFrom(result);
          }
        }
        
        public override pbd::MessageDescriptor DescriptorForType {
          get { return global::Futu.OpenApi.Pb.QotUpdateRT.Response.Descriptor; }
        }
        
        public override Response DefaultInstanceForType {
          get { return global::Futu.OpenApi.Pb.QotUpdateRT.Response.DefaultInstance; }
        }
        
        public override Response BuildPartial() {
          if (resultIsReadOnly) {
            return result;
          }
          resultIsReadOnly = true;
          return result.MakeReadOnly();
        }
        
        public override Builder MergeFrom(pb::IMessage other) {
          if (other is Response) {
            return MergeFrom((Response) other);
          } else {
            base.MergeFrom(other);
            return this;
          }
        }
        
        public override Builder MergeFrom(Response other) {
          if (other == global::Futu.OpenApi.Pb.QotUpdateRT.Response.DefaultInstance) return this;
          PrepareBuilder();
          if (other.HasRetType) {
            RetType = other.RetType;
          }
          if (other.HasRetMsg) {
            RetMsg = other.RetMsg;
          }
          if (other.HasErrCode) {
            ErrCode = other.ErrCode;
          }
          if (other.HasS2C) {
            MergeS2C(other.S2C);
          }
          this.MergeUnknownFields(other.UnknownFields);
          return this;
        }
        
        public override Builder MergeFrom(pb::ICodedInputStream input) {
          return MergeFrom(input, pb::ExtensionRegistry.Empty);
        }
        
        public override Builder MergeFrom(pb::ICodedInputStream input, pb::ExtensionRegistry extensionRegistry) {
          PrepareBuilder();
          pb::UnknownFieldSet.Builder unknownFields = null;
          uint tag;
          string field_name;
          while (input.ReadTag(out tag, out field_name)) {
            if(tag == 0 && field_name != null) {
              int field_ordinal = global::System.Array.BinarySearch(_responseFieldNames, field_name, global::System.StringComparer.Ordinal);
              if(field_ordinal >= 0)
                tag = _responseFieldTags[field_ordinal];
              else {
                if (unknownFields == null) {
                  unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
                }
                ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
                continue;
              }
            }
            switch (tag) {
              case 0: {
                throw pb::InvalidProtocolBufferException.InvalidTag();
              }
              default: {
                if (pb::WireFormat.IsEndGroupTag(tag)) {
                  if (unknownFields != null) {
                    this.UnknownFields = unknownFields.Build();
                  }
                  return this;
                }
                if (unknownFields == null) {
                  unknownFields = pb::UnknownFieldSet.CreateBuilder(this.UnknownFields);
                }
                ParseUnknownField(input, unknownFields, extensionRegistry, tag, field_name);
                break;
              }
              case 8: {
                result.hasRetType = input.ReadInt32(ref result.retType_);
                break;
              }
              case 18: {
                result.hasRetMsg = input.ReadString(ref result.retMsg_);
                break;
              }
              case 24: {
                result.hasErrCode = input.ReadInt32(ref result.errCode_);
                break;
              }
              case 34: {
                global::Futu.OpenApi.Pb.QotUpdateRT.S2C.Builder subBuilder = global::Futu.OpenApi.Pb.QotUpdateRT.S2C.CreateBuilder();
                if (result.hasS2C) {
                  subBuilder.MergeFrom(S2C);
                }
                input.ReadMessage(subBuilder, extensionRegistry);
                S2C = subBuilder.BuildPartial();
                break;
              }
            }
          }
          
          if (unknownFields != null) {
            this.UnknownFields = unknownFields.Build();
          }
          return this;
        }
        
        
        public bool HasRetType {
          get { return result.hasRetType; }
        }
        public int RetType {
          get { return result.RetType; }
          set { SetRetType(value); }
        }
        public Builder SetRetType(int value) {
          PrepareBuilder();
          result.hasRetType = true;
          result.retType_ = value;
          return this;
        }
        public Builder ClearRetType() {
          PrepareBuilder();
          result.hasRetType = false;
          result.retType_ = -400;
          return this;
        }
        
        public bool HasRetMsg {
          get { return result.hasRetMsg; }
        }
        public string RetMsg {
          get { return result.RetMsg; }
          set { SetRetMsg(value); }
        }
        public Builder SetRetMsg(string value) {
          pb::ThrowHelper.ThrowIfNull(value, "value");
          PrepareBuilder();
          result.hasRetMsg = true;
          result.retMsg_ = value;
          return this;
        }
        public Builder ClearRetMsg() {
          PrepareBuilder();
          result.hasRetMsg = false;
          result.retMsg_ = "";
          return this;
        }
        
        public bool HasErrCode {
          get { return result.hasErrCode; }
        }
        public int ErrCode {
          get { return result.ErrCode; }
          set { SetErrCode(value); }
        }
        public Builder SetErrCode(int value) {
          PrepareBuilder();
          result.hasErrCode = true;
          result.errCode_ = value;
          return this;
        }
        public Builder ClearErrCode() {
          PrepareBuilder();
          result.hasErrCode = false;
          result.errCode_ = 0;
          return this;
        }
        
        public bool HasS2C {
         get { return result.hasS2C; }
        }
        public global::Futu.OpenApi.Pb.QotUpdateRT.S2C S2C {
          get { return result.S2C; }
          set { SetS2C(value); }
        }
        public Builder SetS2C(global::Futu.OpenApi.Pb.QotUpdateRT.S2C value) {
          pb::ThrowHelper.ThrowIfNull(value, "value");
          PrepareBuilder();
          result.hasS2C = true;
          result.s2C_ = value;
          return this;
        }
        public Builder SetS2C(global::Futu.OpenApi.Pb.QotUpdateRT.S2C.Builder builderForValue) {
          pb::ThrowHelper.ThrowIfNull(builderForValue, "builderForValue");
          PrepareBuilder();
          result.hasS2C = true;
          result.s2C_ = builderForValue.Build();
          return this;
        }
        public Builder MergeS2C(global::Futu.OpenApi.Pb.QotUpdateRT.S2C value) {
          pb::ThrowHelper.ThrowIfNull(value, "value");
          PrepareBuilder();
          if (result.hasS2C &&
              result.s2C_ != global::Futu.OpenApi.Pb.QotUpdateRT.S2C.DefaultInstance) {
              result.s2C_ = global::Futu.OpenApi.Pb.QotUpdateRT.S2C.CreateBuilder(result.s2C_).MergeFrom(value).BuildPartial();
          } else {
            result.s2C_ = value;
          }
          result.hasS2C = true;
          return this;
        }
        public Builder ClearS2C() {
          PrepareBuilder();
          result.hasS2C = false;
          result.s2C_ = null;
          return this;
        }
      }
      static Response() {
        object.ReferenceEquals(global::Futu.OpenApi.Pb.QotUpdateRT.Descriptor, null);
      }
    }
    
    #endregion
    
  }
}

#endregion Designer generated code
