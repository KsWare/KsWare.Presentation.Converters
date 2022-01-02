using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using JetBrains.Annotations;

namespace KsWare.Presentation.Converters {
	
	/// <summary> Provides a <see cref="Binding"/> for visibility
	/// </summary>
	/// <remarks>
	/// Identically functionality as <see cref="Binding"/> with additional
	/// <see cref="Binding.ValidatesOnDataErrors"/>, 
	/// <see cref="Binding.ValidatesOnExceptions"/> and
	/// <see cref="Binding.UpdateSourceTrigger"/>
	/// set to <c>true</c>.
	/// <p/>
	/// </remarks>
	/// <example>
	/// <code>
	/// &lt;TextBox 
	///     Style="{StaticResource ValidatingTextBox}" 
	/// 	Text="{ext:BindingWithValidation Path=MyBindingPath}" &gt;
	/// &lt;/TextBox&gt;
	/// </code>
	/// </example>
	[MarkupExtensionReturnType(typeof (Visibility))]
	[PublicAPI]
	public class VisibilityBinding:Binding {

		/// <summary> Initializes a new instance of the <see cref="Binding"/> class 
		/// with additional
		/// <see cref="Binding.ValidatesOnDataErrors"/>, 
		/// <see cref="Binding.ValidatesOnExceptions"/> and
		/// <see cref="Binding.UpdateSourceTrigger"/>
		/// set to <c>true</c>..
		/// </summary>
		[Obsolete("Not yet implemented",true)]
		public VisibilityBinding() {
			Mode=BindingMode.OneWay;
		}

		/// <summary> Initializes a new instance of the <see cref="Binding" /> class with an initial path and
		/// with additional
		/// <see cref="Binding.ValidatesOnDataErrors"/>, 
		/// <see cref="Binding.ValidatesOnExceptions"/> and
		/// <see cref="Binding.UpdateSourceTrigger"/>
		/// set to <c>true</c>.
		/// </summary>
		/// <param name="expression"> </param>
		public VisibilityBinding(VisibilityConverter.Expression expression) {
			Mode = BindingMode.OneWay;
			Converter = VisibilityConverter.Get(expression);
			ConverterParameter = new VisibilityConverterParameter();
		}

		/// <summary> Initializes a new instance of the <see cref="Binding" /> class with an initial path and
		/// with additional
		/// <see cref="Binding.ValidatesOnDataErrors"/>, 
		/// <see cref="Binding.ValidatesOnExceptions"/> and
		/// <see cref="Binding.UpdateSourceTrigger"/>
		/// set to <c>true</c>.
		/// </summary>
		/// <param name="path">The initial <see cref="System.Windows.Data.Binding.Path" /> for the binding.</param>
		/// <param name="expression"> </param>
		public VisibilityBinding(string path, VisibilityConverter.Expression expression) : base(path) {
			Mode=BindingMode.OneWay;
			Converter = VisibilityConverter.Get(expression);
			ConverterParameter = new VisibilityConverterParameter();
		}

		/// <summary> Initializes a new instance of the <see cref="Binding" /> class with an initial path and
		/// with additional
		/// <see cref="Binding.ValidatesOnDataErrors"/>, 
		/// <see cref="Binding.ValidatesOnExceptions"/> and
		/// <see cref="Binding.UpdateSourceTrigger"/>
		/// set to <c>true</c>.
		/// </summary>
		/// <param name="path">The initial <see cref="System.Windows.Data.Binding.Path" /> for the binding.</param>
		/// <param name="expression"> </param>
		/// <param name="compareValue"> </param>
		public VisibilityBinding(string path, VisibilityConverter.Expression expression, object compareValue) : base(path) {
			Mode=BindingMode.OneWay;
			Converter = VisibilityConverter.Get(expression);
			ConverterParameter = new VisibilityConverterParameter {CompareValue = compareValue};
		}

		/// <summary> Gets or sets the visibility at design-time.
		/// </summary>
		/// <value>The visibility at design-time.</value>
		[PublicAPI]
		public Visibility? DesignVisibility {
			set {
				if (!(ConverterParameter is VisibilityConverterParameter parameter)) {
					ConverterParameter = parameter=new VisibilityConverterParameter();
				}
				parameter.DesigntimeVisibility = value;
			}
			get {
				return ConverterParameter is VisibilityConverterParameter parameter ? parameter.DesigntimeVisibility : null;
			}
		}

		/// <summary> Gets or sets the compare value used with <see cref="VisibilityConverter.Expression.EqualVisibleElseCollapsed" /> and other compare expressions.
		/// </summary>
		/// <value>The value for compare.</value>
		public object CompareValue {
			set {
				if (!(ConverterParameter is VisibilityConverterParameter parameter)) {
					ConverterParameter = parameter=new VisibilityConverterParameter();
				}
				parameter.CompareValue = value;
			}
			get {
				return ConverterParameter is VisibilityConverterParameter parameter ? parameter.CompareValue : null;
			}
		}
	}

}
