using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.UnityConverters;
using Newtonsoft.Json.UnityConverters.AI.NavMesh;
using Newtonsoft.Json.UnityConverters.Camera;
using Newtonsoft.Json.UnityConverters.Geometry;
using Newtonsoft.Json.UnityConverters.Hashing;
using Newtonsoft.Json.UnityConverters.Math;
using Newtonsoft.Json.UnityConverters.NativeArray;
using Newtonsoft.Json.UnityConverters.Physics;
using Newtonsoft.Json.UnityConverters.Physics2D;
using Newtonsoft.Json.UnityConverters.Random;
using Newtonsoft.Json.UnityConverters.Scripting;
using UnityEngine;
using Object = UnityEngine.Object;
#if ADDRESSABLES
using Newtonsoft.Json.UnityConverters.Addressables;
#endif

namespace Ateo.Common.Util
{
	/// <summary>Helper-class for JSON.</summary>
	public static class JsonHelper
	{
		public static JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			ReferenceLoopHandling = ReferenceLoopHandling.Error,
			MissingMemberHandling = MissingMemberHandling.Ignore,
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
			Formatting = Formatting.None,
			ContractResolver = new UnityTypeContractResolverExtended(),
			Converters = new List<JsonConverter>
			{
				// Unity Converters
#if ADDRESSABLES
				new AssetReferenceConverter(),
#endif
				new NavMeshQueryFilterConverter(),
				new NavMeshTriangulationConverter(),
				new CullingGroupEventConverter(),
				new BoundsConverter(),
				new BoundsIntConverter(),
				new PlaneConverter(),
				new RectConverter(),
				new RectIntConverter(),
				new RectOffsetConverter(),
				new Hash128Converter(),
				new Color32Converter(),
				new ColorConverter(),
				new Matrix4x4Converter(),
				new QuaternionConverter(),
				new SphericalHarmonicsL2Converter(),
				new Vector2Converter(),
				new Vector2IntConverter(),
				new Vector3Converter(),
				new Vector3IntConverter(),
				new Vector4Converter(),
				new NativeArrayConverter(),
				new JointDriveConverter(),
				new JointLimitsConverter(),
				new SoftJointLimitConverter(),
				new ColliderDistance2DConverter(),
				new ContactFilter2DConverter(),
				new RandomStateConverter(),
				new LayerMaskConverter(),
				new RangeIntConverter(),
				new BinaryConverter(),
				
				// JSON Converters
				new DataSetConverter(),
				new DataTableConverter(),
				new DiscriminatedUnionConverter(),
				new EntityKeyMemberConverter(),
				new ExpandoObjectConverter(),
				new IsoDateTimeConverter(),
				new JavaScriptDateTimeConverter(),
				new KeyValuePairConverter(),
				new RegexConverter(),
				new StringEnumConverter(),
				new UnixDateTimeConverter(),
				new VersionConverter(),
				new XmlNodeConverter()
			}
		};
		
		/// <summary>Serialize an object to a JSON string.</summary>
		/// <param name="value">Value to serialize.</param>
		public static string SerializeToString<T>(T value)
		{
			return JsonConvert.SerializeObject(value, Settings);
		}
		
		/// <summary>Serialize an object to a JSON file.</summary>
		/// <param name="value">Value to serialize.</param>
		/// <param name="filename">File name of the JSON.</param>
		public static void SerializeToFile<T>(T value, string filename)
		{
			if (null == value)
			{
				throw new ArgumentNullException(nameof(value));
			}

			if (filename == null)
			{
				throw new ArgumentNullException(nameof(filename));
			}

			try
			{
				File.WriteAllText(filename, SerializeToString(value));
			}
			catch (Exception e)
			{
				Debug.LogError($"JsonHelper.SerializeToFile: {e}");
			}
		}

		/// <summary>Deserializes an JSON string to an object.</summary>
		/// <param name="json">The JSON string to deserialize.</param>
		public static T DeserializeFromString<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json, Settings);
		}
		
		/// <summary>Deserialize a JSON file to an object.</summary>
		/// <param name="filename">JSON file of the object</param>
		public static T DeserializeFromFile<T>(string filename)
		{
			if (filename == null)
			{
				throw new ArgumentNullException(nameof(filename));
			}

			try
			{
				if (File.Exists(filename))
				{
					string data = File.ReadAllText(filename);

					if (string.IsNullOrEmpty(data))
					{
						Debug.LogWarning($"JsonHelper.DeserializeFromFile: File is empty: {filename}");
					}
					else
					{
						return DeserializeFromString<T>(data);
					}
				}
				else
				{
					Debug.LogError($"JsonHelper.DeserializeFromFile: File does not exist: {filename}");
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"JsonHelper.DeserializeFromFile: {e}");
			}

			return default;
		}

		/// <summary>Deserialize a Unity JSON resource (TextAsset) to an object.</summary>
		/// <param name="resourceName">Name of the resource</param>
		public static T DeserializeFromResource<T>(string resourceName)
		{
			if (string.IsNullOrEmpty(resourceName))
			{
				throw new ArgumentNullException(nameof(resourceName));
			}

			// Load the resource
			TextAsset textAsset = Resources.Load(resourceName) as TextAsset;
			return textAsset != null ? DeserializeFromString<T>(textAsset.text) : default;
		}

		/// <summary>
		/// Create a copy of an object.
		/// </summary>
		/// <param name="value">The object to be copied.</param>
		public static T Copy<T>(T value)
		{
			TryCopy(value, out T copy);
			return copy;
		}

		/// <summary>
		/// Attempts to create a copy of an object.
		/// </summary>
		/// <param name="value">The object to be copied.</param>
		/// <param name="copy">The copied object.</param>
		/// <returns>True if a copy was successfully created.</returns>
		public static bool TryCopy<T>(T value, out T copy)
		{
			Type type = typeof(T);

			// Value type -> No copy needed
			if (typeof(ValueType).IsAssignableFrom(type))
			{
				copy = value;
				return true;
			}

			// Inherits from UnityEngine.Object -> No copy possible
			if (typeof(Object).IsAssignableFrom(type))
			{
				copy = value;
				return false;
			}

			// Reference Type -> Create a copy by serializing and deserializing
			copy = DeserializeFromString<T>(SerializeToString(value));
			return true;
		}
		
		/*public static string SerializeToMinimalJson(object obj)
		{
			return JToken.FromObject(obj).RemoveEmptyChildren().ToString();
		}

		public static JToken RemoveEmptyChildren(this JToken token)
		{
			if (token.Type == JTokenType.Object)
			{
				JObject copy = new JObject();
				
				foreach (JProperty prop in token.Children<JProperty>())
				{
					JToken child = prop.Value;
					
					if (child.HasValues)
					{
						child = child.RemoveEmptyChildren();
					}

					if (!child.IsEmptyOrDefault())
					{
						copy.Add(prop.Name, child);
					}
				}

				return copy;
			}
			
			if (token.Type == JTokenType.Array)
			{
				JArray copy = new JArray();
				
				foreach (JToken item in token.Children())
				{
					JToken child = item;
					
					if (child.HasValues)
					{
						child = child.RemoveEmptyChildren();
					}

					if (!child.IsEmptyOrDefault())
					{
						copy.Add(child);
					}
				}

				return copy;
			}

			return token;
		}

		public static bool IsEmptyOrDefault(this JToken token)
		{
			return (token.Type == JTokenType.Array && !token.HasValues) ||
			       (token.Type == JTokenType.Object && !token.HasValues) ||
			       (token.Type == JTokenType.String && token.ToString() == string.Empty) ||
			       (token.Type == JTokenType.Boolean && token.Value<bool>() == false) ||
			       (token.Type == JTokenType.Integer && token.Value<int>() == 0) ||
			       (token.Type == JTokenType.Float && token.Value<double>() == 0.0) ||
			       (token.Type == JTokenType.Null);
		}*/
	}

	internal class UnityTypeContractResolverExtended : UnityTypeContractResolver
	{
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);
			
			if (member.GetCustomAttribute<JsonIgnoreAttribute>() != null)
			{
				jsonProperty.Ignored = true;
			}

			return jsonProperty;
		}
	}
}