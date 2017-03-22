using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Eslon
{
    /// <summary>
    /// Presents a thread-safe aggregation of editors.
    /// </summary>
    public sealed class EslonEngine
    {
        private EslonReader reader;
        private EslonWriter writer;
        private EslonReflector[] reflectors;
        private EslonConduit[] conduits;
        private Dictionary<Type, EslonEnginePrime> primes;
        private List<Frame> stack;
        private int post;

        /// <summary>
        /// Initializes a new instance of <see cref="EslonEngine"/>.
        /// </summary>
        /// <param name="reader">
        /// The reader to employ.
        /// </param>
        /// <param name="writer">
        /// The writer to employ.
        /// </param>
        /// <param name="editors">
        /// The editors to employ.
        /// </param>
        /// <param name="reflectors">
        /// The reflectors to employ.
        /// </param>
        /// <param name="conduits">
        /// The conduits to employ.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <p>
        /// <paramref name="reader"/> is null.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="writer"/> is null.
        /// </p>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <p>
        /// <paramref name="editors"/> contains a null reference.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="reflectors"/> contains a null reference.
        /// </p>
        /// -or-
        /// <p>
        /// <paramref name="conduits"/> contains a null reference.
        /// </p>
        /// </exception>
        public EslonEngine(EslonReader reader, EslonWriter writer, EslonEditor[] editors, EslonReflector[] reflectors, EslonConduit[] conduits)
        {
            API.Check(reader, nameof(reader));
            API.Check(writer, nameof(writer));

            if (editors != null && editors.DetectNull())
            {
                API.ThrowArgumentException("The array contains a null reference.", nameof(editors));
            }

            if (reflectors != null && reflectors.DetectNull())
            {
                API.ThrowArgumentException("The array contains a null reference.", nameof(reflectors));
            }

            if (conduits != null && conduits.DetectNull())
            {
                API.ThrowArgumentException("The array contains a null reference.", nameof(conduits));
            }

            this.reader = reader;
            this.writer = writer;
            this.reflectors = (reflectors == null) ? BCL.Empty<EslonReflector>() : Extra.DupeArray(reflectors);
            this.conduits = (conduits == null) ? BCL.Empty<EslonConduit>() : Extra.DupeArray(conduits);
            this.primes = new Dictionary<Type, EslonEnginePrime>(239);
            this.stack = new List<Frame>();

            Load(editors ?? BCL.Empty<EslonEditor>());
        }

        /// <summary>
        /// Gets the employed reader.
        /// </summary>
        /// <returns>
        /// The employed reader.
        /// </returns>
        public EslonReader Reader
        {
            get { return reader; }
        }

        /// <summary>
        /// Gets the employed writer.
        /// </summary>
        /// <returns>
        /// The employed writer.
        /// </returns>
        public EslonWriter Writer
        {
            get { return writer; }
        }

        /// <summary>
        /// Reads the specified string.
        /// </summary>
        /// <typeparam name="T">
        /// The type to cast.
        /// </typeparam>
        /// <param name="text">
        /// The string to read.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="T"/>, or null if the specified string induces a null object.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="text"/> is null.
        /// </exception>
        /// <exception cref="EslonReadException">
        /// The text induced an error.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The cast is invalid.
        /// </exception>
        /// <seealso cref="EslonEnginePrime.Cast(EslonVolume, System.Object)"/>
        public T Read<T>(string text)
        {
            return Read<T>(text, null);
        }

        /// <summary>
        /// Reads the specified string.
        /// </summary>
        /// <typeparam name="T">
        /// The type to cast.
        /// </typeparam>
        /// <param name="text">
        /// The string to read.
        /// </param>
        /// <param name="parent">
        /// The parent to employ.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="T"/>, or null if the specified string induces a null object.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="text"/> is null.
        /// </exception>
        /// <exception cref="EslonReadException">
        /// The text induced an error.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The cast is invalid.
        /// </exception>
        /// <seealso cref="EslonEnginePrime.Cast(EslonVolume, System.Object)"/>
        public T Read<T>(string text, object parent)
        {
            API.Check(text, nameof(text));

            return Cast<T>(reader.Read(text), parent);
        }

        /// <summary>
        /// Writes the specified object to a new string.
        /// </summary>
        /// <param name="value">
        /// The object to write.
        /// </param>
        /// <returns>
        /// An instance of <see cref="System.String"/>.
        /// </returns>
        /// <seealso cref="EslonEnginePrime.Digest(System.Object)"/>
        public string Write(object value)
        {
            return writer.Append(Digest(value), new StringBuilder(128)).ToString();
        }

        /// <summary>
        /// Writes the specified object.
        /// </summary>
        /// <param name="value">
        /// The object to write.
        /// </param>
        /// <param name="textWriter">
        /// The text writer to employ.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="textWriter"/> is null.
        /// </exception>
        /// <seealso cref="EslonEnginePrime.Digest(System.Object)"/>
        public void Write(object value, TextWriter textWriter)
        {
            API.Check(textWriter, nameof(textWriter));

            writer.Write(Digest(value), textWriter, 0);
        }

        /// <summary>
        /// Casts an object from the specified volume.
        /// </summary>
        /// <typeparam name="T">
        /// The type to cast.
        /// </typeparam>
        /// <param name="volume">
        /// The volume to convert.
        /// </param>
        /// <param name="parent">
        /// The parent to employ.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="T"/>, or null if the specified volume is a null object.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="volume"/> is null.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The cast is invalid.
        /// </exception>
        /// <seealso cref="EslonEnginePrime.Cast(EslonVolume, System.Object)"/>
        public T Cast<T>(EslonVolume volume, object parent)
        {
            API.Check(volume, nameof(volume));

            return (T)Elect(typeof(T)).Cast(volume, parent);
        }

        /// <summary>
        /// Converts the specified object to a volume.
        /// </summary>
        /// <param name="value">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EslonVolume"/>.
        /// </returns>
        /// <seealso cref="EslonEnginePrime.Digest(System.Object)"/>
        public EslonVolume Digest(object value)
        {
            return (value == null) ? new EslonVoid() : Elect(value.GetType()).Digest(value);
        }

        /// <summary>
        /// Collects the editor registration for the specified type.
        /// </summary>
        /// <param name="type">
        /// The type to employ.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EslonEnginePrime"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="type"/> is null.
        /// </exception>
        public EslonEnginePrime Collect(Type type)
        {
            API.Check(type, nameof(type));

            lock (primes)
            {
                return Impend(type) ?? Negate(type, false);
            }
        }

        private EslonEnginePrime Elect(Type type)
        {
            return Arrogate(type) ?? Negate(type, true);
        }

        private EslonEnginePrime Negate(Type type, bool elect)
        {
            EslonEnginePrime prime;

            lock (primes)
            {
                bool initial = (stack.Count == 0);

                if (elect && !initial)
                {
                    throw new EslonEngineException("The entry is invalid.");
                }

                if (!primes.TryGetValue(type, out prime) || !prime.Enabled)
                {
                    if (type.IsAbstract || type.ContainsGenericParameters)
                    {
                        throw new EslonEngineException(COM.Express("Type '%' is not eligible.", type.FullName));
                    }

                    Frame frame = Append(type);
                    EslonEditor editor = null;

                    try
                    {
                        editor = Reflect(type);
                    }
                    finally
                    {
                        frame.Editor = editor;

                        if (initial)
                        {
                            if (editor != null)
                            {
                                Cement();
                            }

                            stack.Clear();
                        }
                    }

                    prime = frame.Prime;
                }
            }

            return prime;
        }

        private EslonEditor Reflect(Type type)
        {
            EslonEditor editor;

            foreach (EslonReflector item in reflectors)
            {
                if ((editor = item.Generate(type)) != null)
                {
                    Check(editor);

                    return editor;
                }
            }

            throw new EslonEngineException(COM.Express("Type '%' is not available.", type.FullName));
        }

        private void Cement()
        {
            if (!Post())
            {
                Spin();
            }

            foreach (Frame item in stack)
            {
                item.Prime.Set(item.Editor);
            }

            post = 0;
        }

        private EslonEnginePrime Impend(Type type)
        {
            foreach (Frame item in stack)
            {
                if (item.Value == type)
                {
                    return item.Prime;
                }
            }

            return null;
        }

        private Frame Append(Type type)
        {
            Frame frame = new Frame()
            {
                Value = type
            };

            if (!Post())
            {
                Spin();
            }

            frame.Prime = Commit(type, null);
            post = 0;
            stack.Add(frame);

            return frame;
        }

        private EslonEnginePrime Arrogate(Type type)
        {
            if (!Post())
            {
                Spin();
            }

            EslonEnginePrime prime;

            if (primes.TryGetValue(type, out prime) && !prime.Enabled)
            {
                prime = null;
            }

            post = 0;

            return prime;
        }

        private void Spin()
        {
            SpinWait wait = new SpinWait();

            do
            {
                wait.SpinOnce();
            }
            while (!Post());
        }

        private bool Post()
        {
            return (Interlocked.CompareExchange(ref post, 1, 0) == 0);
        }

        private void Load(EslonEditor[] editors)
        {
            EnableModules(reflectors);
            EnableModules(conduits);

            foreach (EslonEditor item in editors)
            {
                Check(item);
                Commit(item.Logo, item);
            }
        }

        private void EnableModules(EslonEngineModule[] modules)
        {
            foreach (EslonEngineModule item in modules)
            {
                item.Set(this);
            }
        }

        private void Check(EslonEditor editor)
        {
            if (editor.Reader.GetType() != reader.GetType())
            {
                throw new EslonEngineException(COM.Express("The acquired editor of type '%' contains an invalid reader.", editor.GetType().Name));
            }
        }

        private EslonEnginePrime Commit(Type type, EslonEditor editor)
        {
            EslonEnginePrime prime;

            if (primes.TryGetValue(type, out prime))
            {
                prime.Set(editor);
            }
            else
            {
                prime = new EslonEnginePrime(editor, SelectConduit(type));
                primes.Add(type, prime);
            }

            return prime;
        }

        private EslonConduit SelectConduit(Type type)
        {
            Type[] classes = ExtraReflection.EnumerateClass(type);
            Type[] interfaces = ExtraReflection.EnumerateInterface(type);

            foreach (EslonConduit item in conduits)
            {
                if (item.Logo.IsAbstract)
                {
                    Type relay;

                    if (ExtraReflection.RelateTypeLog(classes, item.Logo, out relay) || ExtraReflection.RelateTypeLog(interfaces, item.Logo, out relay))
                    {
                        return item;
                    }
                }
                else
                {
                    if (ExtraReflection.EquateType(type, item.Logo))
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        private class Frame
        {
            public Type Value;
            public EslonEnginePrime Prime;
            public EslonEditor Editor;
        }
    }

    /// <summary>
    /// Presents an editor registration of <see cref="EslonEngine"/>.
    /// </summary>
    public sealed class EslonEnginePrime
    {
        private EslonEditor editor;
        private EslonConduit conduit;

        internal EslonEnginePrime(EslonEditor editor, EslonConduit conduit)
        {
            this.editor = editor;
            this.conduit = conduit;
        }

        /// <summary>
        /// Gets the assigned editor.
        /// </summary>
        /// <returns>
        /// The assigned editor.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// The prime is disabled.
        /// </exception>
        public EslonEditor Editor
        {
            get
            {
                if (editor == null)
                {
                    API.ThrowInvalidOperationException("The prime is disabled.");
                }

                return editor;
            }
        }

        internal bool Enabled
        {
            get
            {
                return (editor != null);
            }
        }

        /// <summary>
        /// Casts an object from the specified volume.
        /// </summary>
        /// <param name="volume">
        /// The volume to convert.
        /// </param>
        /// <param name="parent">
        /// The parent to employ.
        /// </param>
        /// <returns>
        /// An instance of <see cref="System.Object"/>, or null if the specified volume is a null object.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="volume"/> is null.
        /// </exception>
        /// <exception cref="EslonMoveException">
        /// The cast is invalid.
        /// </exception>
        /// <seealso cref="EslonEditor.Cast(EslonVolume, System.Object)"/>
        public object Cast(EslonVolume volume, object parent)
        {
            API.Check(volume, nameof(volume));

            if (volume.Symbol != EslonSymbol.Null)
            {
                conduit?.Generate(Editor, ref volume);
            }

            return Editor.Cast(volume, parent);
        }

        /// <summary>
        /// Converts the specified object to a volume.
        /// </summary>
        /// <param name="value">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// An instance of <see cref="EslonVolume"/>.
        /// </returns>
        /// <seealso cref="EslonEditor.Digest(System.Object)"/>
        public EslonVolume Digest(object value)
        {
            return Editor.Digest(value);
        }

        internal void Set(EslonEditor value)
        {
            editor = value;
        }
    }

    /// <summary>
    /// Presents a module for <see cref="EslonEngine"/>.
    /// </summary>
    public abstract class EslonEngineModule
    {
        private EslonEngine engine;

        internal EslonEngineModule() { }

        /// <summary>
        /// Gets the associated engine.
        /// </summary>
        /// <returns>
        /// The associated engine.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// The module is not associated with an engine.
        /// </exception>
        public EslonEngine Engine
        {
            get
            {
                if (engine == null)
                {
                    API.ThrowInvalidOperationException("The module is not associated with an engine.");
                }

                return engine;
            }
        }

        internal void Set(EslonEngine value)
        {
            if (engine != null)
            {
                throw new EslonEngineException("The module is already registered.");
            }

            engine = value;
        }
    }

    /// <summary>
    /// The exception that is thrown upon a critical engine error.
    /// </summary>
    public class EslonEngineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EslonEngineException"/>.
        /// </summary>
        /// <param name="message">
        /// The message that describes the exception.
        /// </param>
        public EslonEngineException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of <see cref="EslonEngineException"/>.
        /// </summary>
        /// <param name="message">
        /// The message that describes the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that causes the current exception.
        /// </param>
        public EslonEngineException(string message, Exception innerException) : base(message, innerException) { }
    }
}
