using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using UnityEngine;
using System.Text;
using System.Globalization;

namespace ES3Internal
{
	internal class ES3BinaryWriter : ES3Writer
	{
		internal BinaryWriter baseWriter;

		public ES3BinaryWriter(Stream stream, ES3Settings settings) : this(stream, settings, true, true){}

		internal ES3BinaryWriter(Stream stream, ES3Settings settings, bool writeHeaderAndFooter, bool mergeKeys) : base(settings, writeHeaderAndFooter, mergeKeys)
		{
			baseWriter = new BinaryWriter(stream, settings.encoding);
			StartWriteFile();
		}

		#region WritePrimitive(value) methods.

		internal override void WritePrimitive(int value)		{ baseWriter.Write((byte)ES3SpecialByte.Int);  baseWriter.Write(value); }
		internal override void WritePrimitive(float value)	    { baseWriter.Write((byte)ES3SpecialByte.Float); baseWriter.Write(value); }
		internal override void WritePrimitive(bool value)		{ baseWriter.Write((byte)ES3SpecialByte.Bool); baseWriter.Write(value); }
		internal override void WritePrimitive(decimal value)	{ baseWriter.Write((byte)ES3SpecialByte.Decimal); baseWriter.Write(value); }
		internal override void WritePrimitive(double value)	    { baseWriter.Write((byte)ES3SpecialByte.Double); baseWriter.Write(value); }
		internal override void WritePrimitive(long value)		{ baseWriter.Write((byte)ES3SpecialByte.Long); baseWriter.Write(value); }
		internal override void WritePrimitive(ulong value)	    { baseWriter.Write((byte)ES3SpecialByte.Ulong); baseWriter.Write(value); }
		internal override void WritePrimitive(uint value)		{ baseWriter.Write((byte)ES3SpecialByte.Uint); baseWriter.Write(value); }
		internal override void WritePrimitive(byte value)		{ baseWriter.Write((byte)ES3SpecialByte.Byte); baseWriter.Write(value); }
		internal override void WritePrimitive(sbyte value)	    { baseWriter.Write((byte)ES3SpecialByte.Sbyte); baseWriter.Write(value); }
		internal override void WritePrimitive(short value)	    { baseWriter.Write((byte)ES3SpecialByte.Short); baseWriter.Write(value); }
		internal override void WritePrimitive(ushort value)	    { baseWriter.Write((byte)ES3SpecialByte.Ushort); baseWriter.Write(value); }
		internal override void WritePrimitive(char value)		{ baseWriter.Write((byte)ES3SpecialByte.Char); baseWriter.Write(value); }
		internal override void WritePrimitive(byte[] value)		{ baseWriter.Write((byte)ES3SpecialByte.ByteArray); baseWriter.Write(value.Length);  baseWriter.Write(value); }
		internal override void WritePrimitive(string value)     { baseWriter.Write((byte)ES3SpecialByte.String); baseWriter.Write(value);  }

		internal override void WriteNull()
		{
			baseWriter.Write((byte)ES3SpecialByte.Null);
		}

		#endregion

		#region Overridden methods

		internal override void WriteRawProperty(string name, byte[] value)
		{ 
			StartWriteProperty(name); baseWriter.Write(value); EndWriteProperty(name);
		}


        // File

		internal override void StartWriteFile()
        { 
            base.StartWriteFile(); 
        }
        internal override void EndWriteFile()
        { 
            base.EndWriteFile(); 
        }


        // Property

		internal override void StartWriteProperty(string name)
		{
            base.StartWriteProperty(name);
			WritePrimitive(name);
        }

		internal override void EndWriteProperty(string name)
		{
            base.EndWriteProperty(name);
        }


        // Object

		internal override void StartWriteObject(string name)
		{
            base.StartWriteObject(name);
            baseWriter.Write((byte)ES3SpecialByte.Object);
        }

		internal override void EndWriteObject(string name)
		{
            baseWriter.Write(ES3Binary.ObjectTerminator);
            base.EndWriteObject(name);
        }


        // Collection

		internal override void StartWriteCollection()
		{
            base.StartWriteCollection();
            baseWriter.Write((byte)ES3SpecialByte.Collection);
        }

		internal override void EndWriteCollection()
		{
            baseWriter.Write((byte)ES3SpecialByte.Terminator);
            base.EndWriteCollection();
		}

		internal override void StartWriteCollectionItem(int index){}

		internal override void EndWriteCollectionItem(int index)
        { 
            baseWriter.Write((byte)ES3SpecialByte.CollectionItem); 
        }


        // Dictionary

		internal override void StartWriteDictionary()
		{
			StartWriteObject(null);
            baseWriter.Write((byte)ES3SpecialByte.Dictionary);
        }

		internal override void EndWriteDictionary()
		{
            baseWriter.Write((byte)ES3SpecialByte.Terminator);
            EndWriteObject(null);
		}

		internal override void StartWriteDictionaryKey(int index){}
		internal override void EndWriteDictionaryKey(int index){}
		internal override void StartWriteDictionaryValue(int index){}
		internal override void EndWriteDictionaryValue(int index){}

        #endregion

        #region Binary-specific methods


        #endregion

        public override void Dispose()
		{
            #if NETFX_CORE
            baseWriter.Dispose();
            #else
            baseWriter.Close();
            #endif
        }
    }
}
