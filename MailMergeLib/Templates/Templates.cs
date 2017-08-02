using System.Collections.Generic;
using System.Linq;
using YAXLib;

namespace MailMergeLib.Templates
{
    /// <summary>
    /// The class is a container for a list of type <see cref="Template"/>.
    /// </summary>
    /// <code>
    /// var tp = new Templates();
    /// var countries = new List&lt;Part&gt;()
    /// {
    ///    new Part(PartType.Plain, "Germany", "German content"),
    ///    new Part(PartType.Plain,"Austria", "Austrian content"),
    ///    new Part(PartType.Plain, "USA", "US American content"),
    ///    new Part(PartType.Plain, "Canada", "Canadian content"),
    ///    new Part(PartType.Plain, "Brazil", "Brazilian content")
    /// };
    /// tp.Add(new Template() {Id = "Country", DefaultKey = "Brazil", Text = countries } );
    /// // Get the content for a specific country:
    /// var countryContent = tp["Country"]["USA"]; // evaluating the given key only
    /// countryContent = tp["Country"].GetParts("Canada"); // evaluating given key and DefaultKey
    /// </code>
    /// <c>XML representation of the serialized Templates:</c>
    /// <code>
    /// &lt;Templates&gt;
    ///  &lt;Template Name="Country"&gt;
    ///    &lt;Text DefaultKey="Brazil"&gt;
    ///      &lt;Part Key="Germany" Type="Plain"&gt;German content&lt;/Part&gt;
    ///      &lt;Part Key="Germany" Type="Html"&gt;German HTML content&lt;/Part&gt;
    ///      &lt;Part Key="Austria" Type="Plain"&gt;Austrian content&lt;/Part&gt;
    ///      &lt;Part Key="USA" Type="Plain"&gt;US American content&lt;/Part&gt;
    ///      &lt;Part Key="Canada" Type="Plain"&gt;Canadian content&lt;/Part&gt;
    ///      &lt;Part Key="Brazil" Type="Plain"&gt;Brazilian content&lt;/Part&gt;
    ///    &lt;/Text&gt;
    ///  &lt;/Template&gt;
    /// &lt;/Templates&gt;
    /// </code>
    [YAXSerializeAs("Templates")]
    public class Templates : List<Template>
    {
        /// <summary>
        /// Creates an instance of a TestParts class.
        /// </summary>
        public Templates() { } // CTOR needed for deserialization

        /// <summary>
        /// Gets the <see cref="Template"/> for the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="TemplateException"></exception>
        public Template this[string name]
        {
            get { return Find(k => k.Name == name); }

            set
            {
                var tp = Find(k => k.Name == name);
                tp.Name = value.Name;
                tp.Text = value.Text;
                tp.DefaultKey = value.DefaultKey;
            }
        }

        /// <summary>
        /// Adds an object to the end of the list.
        /// </summary>
        /// <param name="newItem"></param>
        public new void Add(Template newItem)
        {
            ThrowIfPartAlreadyExists(newItem);
            base.Add(newItem);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the list.
        /// </summary>
        /// <param name="newItems"></param>
        public new void AddRange(IEnumerable<Template> newItems)
        {
            var ni = newItems.ToArray();
            foreach (var item in ni)
            {
                ThrowIfPartAlreadyExists(item);
                Add(item);
            }
        }

        private void ThrowIfPartAlreadyExists(Template newItem)
        {
            if (this.Any(part => part.Name == newItem.Name))
            {
                throw new TemplateException($"Duplicate entry for a text part with Id '{newItem.Name}'.", null, null, newItem, this);
            }
        }

        /// <summary>
        /// Serialize this object to XML.
        /// </summary>
        /// <returns>Returns a string with XML markup.</returns>
        public string Serialize()
        {
            var serializer = new YAXSerializer(typeof(Templates));
            return serializer.Serialize(this);
        }

        /// <summary>
        /// Deserialize the parameter with XML markup to an instance of <see cref="Templates"/>.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>Returns an instance of <see cref="Templates"/>.</returns>
        public static Templates Deserialize(string xml)
        {
            var serializer = new YAXSerializer(typeof(Templates));
            return (Templates) serializer.Deserialize(xml);
        }

        /// <summary>
        /// Compares the Templates with an other instance of Templates for equality.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Returns true, if both instances are equal, else false.</returns>
        public bool Equals(Templates other)
        {
            // not any entry missing in this, nor in the other list
            return !this.Except(other).Union(other.Except(this)).Any();
        }
    }
}