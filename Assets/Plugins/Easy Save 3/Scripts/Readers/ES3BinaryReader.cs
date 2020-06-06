using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using ES3Types;
using System.Globalization;

namespace ES3Internal
{
	/*
	 * 	Specific ES3Reader for reading Binary data.
	 * 
	 */
	public class ES3BinaryReader : ES3Reader
	{
		public BinaryReader baseReader;

		internal ES3BinaryReader(Stream stream, ES3Settings settings, bool readHeaderAndFooter = true) : base(settings, readHeaderAndFooter)
		{
			this.baseReader = new BinaryReader(stream, settings.encoding);
		}

		#region Property/Key Methods

		/*
		 * 	Reads the name of a property, and must be positioned immediately before a property name.
		 */
		public override string ReadPropertyName()
		{
			var propertyName = Read_string();
			if(propertyName == null)
				throw new FormatException("Stream isn't positioned before a property.");
            ES3Debug.Log("<b>"+propertyName+"</b> (reading property)", null, serializationDepth);
			return propertyName;
		}

		/*
		 * 	Reads the type data prefixed to this key.
		 * 	If ignore is true, it will return null to save the computation of converting
		 * 	the string to a Type.
		 */
		protected override Type ReadKeyPrefix(bool ignoreType=false)
		{
			StartReadObject();

			Type dataType = null;

			string propertyName = ReadPropertyName();
			if(propertyName == ES3Type.typeFieldName)
			{
				string typeString = Read_string();
				dataType = ignoreType ? null : Type.GetType(typeString);
				propertyName  = ReadPropertyName();
			}
				
			if(propertyName != "value")
				throw new FormatException("This data is not Easy Save Key Value data. Expected property name \"value\", found \""+propertyName+"\".");

			return dataType;
		}

		protected override void ReadKeySuffix(){}
		internal override bool StartReadObject(){ return base.StartReadObject(); }
		internal override void EndReadObject(){ base.EndReadObject(); }
		internal override bool StartReadDictionary(){ return true; }
        internal override void EndReadDictionary(){}
		internal override bool StartReadDictionaryKey(){ return true; }
		internal override void EndReadDictionaryKey(){}
		internal override void StartReadDictionaryValue(){}
		internal override bool EndReadDictionaryValue(){ return true; }
		internal override bool StartReadCollection(){ return true; }
		internal override void EndReadCollection(){}
		internal override bool StartReadCollectionItem(){ return true; }
		internal override bool EndReadCollectionItem(){ return true; }

		#endregion

		#region Seeking Methods

		/*
		 * 	Resets the stream and seeks to the given key.
		 */
		internal override bool Goto(string key)
		{
			if(settings.encryptionType == ES3.EncryptionType.None && settings.compressionType == ES3.CompressionType.None)
				Reset();

			string currentKey;
			while((currentKey = ReadPropertyName()) != key)
			{
				if(currentKey == null)
					return false;
				Skip();
			}
			return true;
		}

		/* Resets the stream back to the beginning, resetting any buffers. */
		protected void Reset()
		{
			// If we're already at the beginning of the stream, do nothing.
			if(baseReader.BaseStream.Position == 0)
				return;
		}

        internal override byte[] ReadElement(bool skip = false)
        {
            using (var writer = skip ? null : new BinaryWriter(new MemoryStream(settings.bufferSize)))
            {
                ReadElement(writer, skip);
                if (skip)
                    return null;
                writer.Flush();
                return ((MemoryStream)writer.BaseStream).ToArray();
            }
        }

        /*
		 * 	Reads the current object in the stream.
		 * 	Stream position should be immediately before the type.
		 */
        private void ReadElement(BinaryWriter writer, bool skip=false)
		{
            byte b = (byte)baseReader.Read();
            var typeByte = (ES3SpecialByte)b;
            if(!skip)
                writer.Write(b);

            if(ES3Binary.IsPrimitive(typeByte))
            {
                switch (typeByte)
                {
                    case ES3SpecialByte.Bool:
                        if (skip) baseReader.ReadBoolean(); else writer.Write(baseReader.ReadBoolean());
                        break;
                    case ES3SpecialByte.Byte:
                        if (skip) baseReader.ReadByte(); else writer.Write(baseReader.ReadByte());
                        break;
                    case ES3SpecialByte.Sbyte:
                        if (skip) baseReader.ReadSByte(); else writer.Write(baseReader.ReadSByte());
                        break;
                    case ES3SpecialByte.Char:
                        if (skip) baseReader.ReadChar(); else writer.Write(baseReader.ReadChar());
                        break;
                    case ES3SpecialByte.Decimal:
                        if (skip) baseReader.ReadDecimal(); else writer.Write(baseReader.ReadDecimal());
                        break;
                    case ES3SpecialByte.Double:
                        if (skip) baseReader.ReadDecimal(); else writer.Write(baseReader.ReadDecimal());
                        break;
                    case ES3SpecialByte.Float:
                        if (skip) baseReader.ReadSingle(); else writer.Write(baseReader.ReadSingle());
                        break;
                    case ES3SpecialByte.Int:
                        if (skip) baseReader.ReadInt32(); else writer.Write(baseReader.ReadInt32());
                        break;
                    case ES3SpecialByte.Uint:
                        if (skip) baseReader.ReadUInt32(); else writer.Write(baseReader.ReadUInt32());
                        break;
                    case ES3SpecialByte.Long:
                        if (skip) baseReader.ReadInt64(); else writer.Write(baseReader.ReadInt64());
                        break;
                    case ES3SpecialByte.Ulong:
                        if (skip) baseReader.ReadUInt64(); else writer.Write(baseReader.ReadUInt64());
                        break;
                    case ES3SpecialByte.Short:
                        if (skip) baseReader.ReadInt16(); else writer.Write(baseReader.ReadInt16());
                        break;
                    case ES3SpecialByte.Ushort:
                        if (skip) baseReader.ReadUInt16(); else writer.Write(baseReader.ReadUInt16());
                        break;
                    case ES3SpecialByte.String:
                        if (skip) baseReader.ReadString(); else writer.Write(baseReader.ReadString());
                        break;
                    case ES3SpecialByte.ByteArray:
                        if (skip) { baseReader.ReadBytes(baseReader.ReadInt32()); } else { writer.Write(baseReader.ReadBytes(baseReader.ReadInt32())); }
                        break;
                }
            }
            else if(typeByte == ES3SpecialByte.Object)
            {
                string propertyName;
                while((propertyName = baseReader.ReadString()) != ES3Binary.ObjectTerminator)
                {
                    if (!skip)
                        writer.Write(propertyName);
                    ReadElement(writer, skip);
                }
            }
            else if(typeByte == ES3SpecialByte.Collection)
            {
                while ((typeByte = (ES3SpecialByte)baseReader.ReadByte()) != ES3SpecialByte.Terminator)
                {
                    if (!skip)
                        writer.Write((byte)typeByte);
                    ReadElement(writer, skip);
                }
            }
		}

		#endregion

		#region Primitive Read() Methods.

        public void ReadIntoWriter(ES3Writer writer)
        {

        }

        internal override long Read_ref()
        {
            if (ES3ReferenceMgr.Current == null)
                throw new InvalidOperationException("An Easy Save 3 Manager is required to load references. To add one to your scene, exit playmode and go to Assets > Easy Save 3 > Add Manager to Scene");
            return baseReader.ReadInt64();
        }

        internal override string    Read_string()   { return baseReader.ReadString(); }
        internal override char		Read_char()		{ return baseReader.ReadChar();     }
		internal override float		Read_float()	{ return baseReader.ReadSingle();   }
		internal override int 		Read_int()		{ return baseReader.ReadInt32();    }
		internal override bool 		Read_bool()		{ return baseReader.ReadBoolean(); 	}
		internal override decimal 	Read_decimal()	{ return baseReader.ReadDecimal(); 	}
		internal override double 	Read_double()	{ return baseReader.ReadDouble(); 	}
		internal override long 		Read_long()		{ return baseReader.ReadInt64();	}
		internal override ulong 	Read_ulong()	{ return baseReader.ReadUInt64();	}
		internal override uint 		Read_uint()		{ return baseReader.ReadUInt32(); 	}
		internal override byte 		Read_byte()		{ return baseReader.ReadByte(); 	}
		internal override sbyte 	Read_sbyte()	{ return baseReader.ReadSByte(); 	}
		internal override short 	Read_short()	{ return baseReader.ReadInt16(); 	}
		internal override ushort 	Read_ushort()	{ return baseReader.ReadUInt16(); 	}
		internal override byte[] 	Read_byteArray(){ return baseReader.ReadBytes(baseReader.ReadInt32()); }

        #endregion

        #region Binary-specific methods

        #endregion


        public override void Dispose()
		{
            #if NETFX_CORE
            baseReader.Dispose();
            #else
            baseReader.Close();
            #endif
        }
    }
}