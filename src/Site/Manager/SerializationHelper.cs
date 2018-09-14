using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;
using System.Linq;

namespace SubtitlesLearn.Manager
{
	/// <summary>
	/// Вспомогательный класс для упрощения сериализации/десериализации объектов.
	/// </summary>
	public static class SerializationHelper
	{
		#region Методы

		/// <summary>
		/// Serizlizes object to json string.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string SerializeJson(object obj)
		{
			//Create a stream to serialize the object to.  
			MemoryStream ms = new MemoryStream();

			// Serializer the User object to the stream.  
			DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
			ser.WriteObject(ms, obj);
			byte[] json = ms.ToArray();
			ms.Close();
			return Encoding.UTF8.GetString(json, 0, json.Length);
		}

		/// <summary>
		/// Deserializes object from json.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="json"></param>
		/// <returns></returns>
		public static T DeserializeJson<T>(string json)
			where T : class
		{
			MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
			DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
			T result = (T)ser.ReadObject(ms);
			ms.Close();
			return result;
		}

		/// <summary>
		/// Сериализация объекта в строку.
		/// <para>namespace - пустые.</para>
		/// </summary>
		/// <param name="obj">Объект, который необходимо сериализовать.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static string Serialize(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			using (StringWriter sw = new StringWriter())
			{
				XmlSerializer ser = new XmlSerializer(obj.GetType());
				XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
				ns.Add(string.Empty, string.Empty);
				ser.Serialize(sw, obj, ns);
				return sw.ToString();
			}
		}

		/// <summary>
		/// Сериализация объекта в строку.
		/// <para>namespace - пустые.</para>
		/// </summary>
		/// <param name="obj">Объект, который необходимо сериализовать.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static string Serialize(object obj, Type[] extraTypes)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			using (StringWriter sw = new StringWriter())
			{
				XmlSerializer ser = new XmlSerializer(obj.GetType(), extraTypes);
				XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
				ns.Add(string.Empty, string.Empty);
				ser.Serialize(sw, obj, ns);
				return sw.ToString();
			}
		}

		/// <summary>
		/// Десериализует объект нужного типа.
		/// </summary>
		/// <typeparam name="T">Тип,экземпляры которого можно сериализовать/десериализовать.</typeparam>
		/// <param name="xml">Xml с данными.</param>
		/// <returns>Десериализованный объект.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static T Deserialize<T>(string xml)
			where T : class
		{
			if (string.IsNullOrEmpty(xml))
				throw new ArgumentNullException("xml");

			XmlSerializer ser = new XmlSerializer(typeof(T));

			using (StringReader reader = new StringReader(xml))
			{
				return (T)ser.Deserialize(reader);
			}
		}

		/// <summary>
		/// Воссоздает объект из его представления, сериализованного в xml.
		/// </summary>
		/// <param name="type">Тип объекта.</param>
		/// <param name="xml">Строка, содержащая сериализованный в xml объект.</param>
		/// <returns>Объект.</returns>
		public static object Deserialize(Type type, string xml)
		{
			object obj = null;

			if (!string.IsNullOrEmpty(xml))
			{
				try
				{
					XmlSerializer xmlObject = new XmlSerializer(type);

					using (StringReader reader = new StringReader(xml))
					{
						obj = xmlObject.Deserialize(reader);
					}
				}
				catch
				{
				}
			}

			if (obj == null)
				obj = Activator.CreateInstance(type);

			return obj;
		}

		#endregion Методы
	}
}
