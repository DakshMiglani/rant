﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rant.IO.Bson
{
    /// <summary>
    /// One item in a BSON document.
    /// </summary>
    public class BsonItem
    {
        private Dictionary<string, BsonItem> _objectValues;
        private object _value;
        private bool _typeSet = false;
        private byte _type = 0;

        /// <summary>
        /// The BSON type code of this object. If not explicitly specified, it will guess.
        /// </summary>
        public byte Type
        {
            get
            {
                if (!_typeSet)
                    DetermineType();

                return _type;
            }
            set
            {
                _typeSet = true;
                _type = value;
            }
        }

        /// <summary>
        /// The value of this item, if it is not a document or array.
        /// </summary>
        public object Value
        {
            get { return _value ?? _objectValues; }
            set
            {
                _value = value;
                DetermineType();
                PossiblyCreateArray();
            }
        }

        /// <summary>
        /// Whether or not this item has sub values (is a document or array).
        /// </summary>
        public bool HasValues
        {
            get { return _value == null; }
        }

        /// <summary>
        /// Creates a new BsonItem with the specified value.
        /// </summary>
        /// <param name="value">The value of this BSON item.</param>
        public BsonItem(object value)
        {
            _value = value;
            _objectValues = new Dictionary<string, BsonItem>();
            PossiblyCreateArray();
        }

        /// <summary>
        /// Converts this double to a BsonItem.
        /// </summary>
        /// <param name="a">The double to convert.</param>
        public static implicit operator BsonItem(double a)
        {
            return new BsonItem(a);
        }

        /// <summary>
        /// Converts this string to a BsonItem.
        /// </summary>
        /// <param name="a">The string to convert.</param>
        public static implicit operator BsonItem(string a)
        {
            return new BsonItem(a);
        }

        /// <summary>
        /// Converts this bool to a BsonItem.
        /// </summary>
        /// <param name="a">The bool to convert.</param>
        public static implicit operator BsonItem(bool a)
        {
            return new BsonItem(a);
        }

        /// <summary>
        /// Converts this int to a BsonItem.
        /// </summary>
        /// <param name="a">The int to convert.</param>
        public static implicit operator BsonItem(int a)
        {
            return new BsonItem(a);
        }

        /// <summary>
        /// Converts this long to a BsonItem.
        /// </summary>
        /// <param name="a">The long to convert.</param>
        public static implicit operator BsonItem(long a)
        {
            return new BsonItem(a);
        }

        /// <summary>
        /// Creates a new empty BsonItem.
        /// </summary>
        public BsonItem()
        {
            _value = null;
            _objectValues = new Dictionary<string, BsonItem>();
        }

        /// <summary>
        /// Checks whether or not this item has the specified key.
        /// </summary>
        /// <param name="key">The key to check for.</param>
        /// <returns>Whether or not this item has the specified key.</returns>
        public bool HasKey(string key)
        {
            return _objectValues.ContainsKey(key);
        }

        /// <summary>
        /// The number of keys in this BsonItem.
        /// </summary>
        public int KeyCount => _objectValues.Keys.Count;

        /// <summary>
        /// An array of the keys in this BsonItem.
        /// </summary>
        public string[] Keys => _objectValues.Keys.ToArray();

        /// <summary>
        /// Accesses the value of the specified key.
        /// </summary>
        /// <param name="key">The key whose value will be accessed.</param>
        /// <returns>The value of the specified key.</returns>
        public BsonItem this[string key]
        {
            get { return _objectValues[key]; }
            set { _objectValues[key] = value; }
        }

        /// <summary>
        /// Accesses the value of the specified key.
        /// </summary>
        /// <param name="key">The key whose value will be accessed.</param>
        /// <returns>The value of the specified key.</returns>
        public BsonItem this[int key]
        {
            get { return _objectValues[key.ToString()]; }
            set { _objectValues[key.ToString()] = value; }
        }

        private void PossiblyCreateArray()
        {
            if (_value == null) return;
            var type = _value.GetType();
            if (!type.IsArray) return;
            var arr = (object[])_value;
            _type = 0x04; // array
            _typeSet = true;
            _objectValues = new Dictionary<string, BsonItem>();
            _value = null;
            for (var i = 0; i < arr.Length; i++)
            {
                _objectValues[i.ToString()] = (
                    arr[i] is BsonItem ?
                        (BsonItem)arr[i] :
                        new BsonItem(arr[i])
                    );
            }
        }

        private void DetermineType()
        {
            if(_value == null)
            {
                _type = 0x03; // document
                return;
            }

            // i apologize for this if statement

            if (_value is double || _value is float)
                _type = 0x01;
            else if (_value is string)
                _type = 0x02;
            else if (_value is byte[])
                _type = 0x05;
            else if (_value is bool)
                _type = 0x08;
            else if (_value is long)
                _type = 0x09;
            else if (_value is int)
                _type = 0x10;
            else
                _type = 0x00;
        }
    }
}