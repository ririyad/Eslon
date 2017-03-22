using System;

namespace Eslon
{
    /// <summary>
    /// Presents an engine module to devise editors.
    /// </summary>
    /// <seealso cref="EslonEngine"/>
    public abstract class EslonReflector : EslonEngineModule
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EslonReflector"/>.
        /// </summary>
        protected EslonReflector() { }

        /// <summary>
        /// Creates an activator for the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to employ.
        /// </param>
        /// <returns>
        /// An instance of <see cref="System.Func{TResult}"/>, or null if the specified type does not contain an eligible constructor.
        /// </returns>
        /// <remarks>
        /// The specified type requires a public and parameterless constructor.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="type"/> is null.
        /// </exception>
        public static Func<object> CreateActivator(Type type)
        {
            API.Check(type, nameof(type));

            PanActivator activator = PanDynamic.CreateActivator(type);

            return (activator == null) ? null : new Func<object>(activator);
        }

        /// <summary>
        /// Determines if the specified type relates to a sign.
        /// </summary>
        /// <param name="type">
        /// The type to relate.
        /// </param>
        /// <param name="signs">
        /// The signs to employ.
        /// </param>
        /// <param name="relay">
        /// Contains a reduction of the specified type, or null if the specified type does not yield.
        /// </param>
        /// <returns>
        /// The zero-based index of the related sign, or –1 if the specified type does not yield.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <p>
        /// <paramref name="type"/> is null.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="signs"/> is null.
        /// </p>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="signs"/> contains a null reference.
        /// </exception>
        public static int RelateClass(Type type, Type[] signs, out Type relay)
        {
            API.Check(type, nameof(type));
            API.Check(signs, nameof(signs));

            if (signs.DetectNull())
            {
                API.ThrowArgumentException("The array contains a null reference.", nameof(signs));
            }

            return ExtraReflection.RelateClass(type, signs, out relay);
        }

        /// <summary>
        /// Determines if the specified type relates to a sign.
        /// </summary>
        /// <param name="type">
        /// The type to relate.
        /// </param>
        /// <param name="signs">
        /// The signs to employ.
        /// </param>
        /// <param name="relay">
        /// Contains a reduction of the specified type, or null if the specified type does not yield.
        /// </param>
        /// <returns>
        /// The zero-based index of the related sign, or –1 if the specified type does not yield.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <p>
        /// <paramref name="type"/> is null.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="signs"/> is null.
        /// </p>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="signs"/> contains a null reference.
        /// </exception>
        public static int RelateInterface(Type type, Type[] signs, out Type relay)
        {
            API.Check(type, nameof(type));
            API.Check(signs, nameof(signs));

            if (signs.DetectNull())
            {
                API.ThrowArgumentException("The array contains a null reference.", nameof(signs));
            }

            return ExtraReflection.RelateInterface(type, signs, out relay);
        }

        /// <summary>
        /// Devises an editor for the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to employ.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EslonEditor"/>, or null if the reflector does not entail the specified type.
        /// </returns>
        protected abstract EslonEditor Elect(Type type);

        internal EslonEditor Generate(Type type)
        {
            return Elect(type);
        }
    }
}
