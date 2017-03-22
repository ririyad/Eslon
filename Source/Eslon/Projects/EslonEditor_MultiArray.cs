using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Eslon
{
    internal class EslonEditor_MultiArray<T> : EslonEditor
    {
        private static readonly Type[] CasterParameterTypes = new Type[] { typeof(int[]) };
        private static readonly Type[] CopierParameterTypes = new Type[] { typeof(Array), typeof(Array), typeof(int[]) };

        private int meter;
        private Caster caster;
        private Copier extractor;
        private Copier inserter;
        private EslonEnginePrime element;

        public EslonEditor_MultiArray(Type logo, EslonReader reader, EslonWriter writer, EslonEnginePrime element) : base(logo, reader, writer)
        {
            this.meter = logo.GetArrayRank();
            this.caster = CreateArrayCaster(logo, logo.GetArrayRank());
            this.extractor = CreateArrayExtractor(logo, logo.GetArrayRank());
            this.inserter = CreateArrayInserter(logo, logo.GetArrayRank());
            this.element = element;
        }

        protected override object AutoCast(EslonVolume volume, object parent)
        {
            EslonList list = volume.Move<EslonList>();

            Array array = caster.Invoke(Measure(list));

            Distribute(list, array, new T[array.GetLength(meter - 1)], new int[meter - 1], 0);

            return array;
        }

        protected override EslonVolume AutoDigest(object value)
        {
            Array array = (Array)value;

            return Digest(array, new T[array.GetLength(meter - 1)], new int[meter - 1], 0);
        }

        private static Caster CreateArrayCaster(Type type, int meter)
        {
            DynamicMethod method = new DynamicMethod(String.Empty, typeof(Array), CasterParameterTypes, typeof(EslonEditor_MultiArray<T>), true);
            ILGenerator gen = method.GetILGenerator();

            for (int i = 0; i < meter; i++)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldc_I4, i);
                gen.Emit(OpCodes.Ldelem_I4);
            }

            gen.Emit(OpCodes.Newobj, type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Extra.CreateArray(meter, typeof(int)), null));
            gen.Emit(OpCodes.Ret);

            return (Caster)method.CreateDelegate(typeof(Caster));
        }

        private static Copier CreateArrayExtractor(Type type, int meter)
        {
            DynamicMethod method = new DynamicMethod(String.Empty, null, CopierParameterTypes, typeof(EslonEditor_MultiArray<T>), true);
            ILGenerator gen = method.GetILGenerator();

            gen.DeclareLocal(typeof(int));
            gen.DeclareLocal(typeof(int));
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldc_I4, meter - 1);
            gen.Emit(OpCodes.Callvirt, type.GetMethod("GetLength", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(int) }, null));
            gen.Emit(OpCodes.Stloc_0);

            int dim = meter - 1;

            for (int i = 0; i < dim; i++)
            {
                gen.DeclareLocal(typeof(int));
                gen.Emit(OpCodes.Ldarg_2);
                gen.Emit(OpCodes.Ldc_I4, i);
                gen.Emit(OpCodes.Ldelem_I4);
                gen.Emit(OpCodes.Stloc, 2 + i);
            }

            Label label1 = gen.DefineLabel();
            Label label2 = gen.DefineLabel();

            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Stloc_1);
            gen.MarkLabel(label1);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Bge, label2);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ldarg_0);

            for (int i = 0; i < dim; i++)
            {
                gen.Emit(OpCodes.Ldloc, 2 + i);
            }

            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Callvirt, type.GetMethod("Get", BindingFlags.Instance | BindingFlags.Public, null, Extra.CreateArray(meter, typeof(int)), null));
            gen.Emit(OpCodes.Stelem, typeof(T));
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Add);
            gen.Emit(OpCodes.Stloc_1);
            gen.Emit(OpCodes.Br, label1);
            gen.MarkLabel(label2);
            gen.Emit(OpCodes.Ret);

            return (Copier)method.CreateDelegate(typeof(Copier));
        }

        private static Copier CreateArrayInserter(Type type, int meter)
        {
            DynamicMethod method = new DynamicMethod(String.Empty, null, CopierParameterTypes, typeof(EslonEditor_MultiArray<T>), true);
            ILGenerator gen = method.GetILGenerator();

            gen.DeclareLocal(typeof(int));
            gen.DeclareLocal(typeof(int));
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldc_I4, meter - 1);
            gen.Emit(OpCodes.Callvirt, type.GetMethod("GetLength", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(int) }, null));
            gen.Emit(OpCodes.Stloc_0);

            int dim = meter - 1;

            for (int i = 0; i < dim; i++)
            {
                gen.DeclareLocal(typeof(int));
                gen.Emit(OpCodes.Ldarg_2);
                gen.Emit(OpCodes.Ldc_I4, i);
                gen.Emit(OpCodes.Ldelem_I4);
                gen.Emit(OpCodes.Stloc, 2 + i);
            }

            Label label1 = gen.DefineLabel();
            Label label2 = gen.DefineLabel();

            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Stloc_1);
            gen.MarkLabel(label1);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Bge, label2);
            gen.Emit(OpCodes.Ldarg_0);

            for (int i = 0; i < dim; i++)
            {
                gen.Emit(OpCodes.Ldloc, 2 + i);
            }

            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ldelem, typeof(T));
            gen.Emit(OpCodes.Callvirt, type.GetMethod("Set", BindingFlags.Instance | BindingFlags.Public, null, GetSetMethodParameterTypes(meter), null));
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Add);
            gen.Emit(OpCodes.Stloc_1);
            gen.Emit(OpCodes.Br, label1);
            gen.MarkLabel(label2);
            gen.Emit(OpCodes.Ret);

            return (Copier)method.CreateDelegate(typeof(Copier));
        }

        private static Type[] GetSetMethodParameterTypes(int meter)
        {
            Type[] array = Extra.CreateArray(meter + 1, typeof(int));

            array[meter] = typeof(T);

            return array;
        }

        private void Distribute(EslonList list, Array array, T[] buffer, int[] indices, int level)
        {
            int length = array.GetLength(level);

            if (list.Count != length)
            {
                throw new EslonMoveException(list, Logo);
            }

            if (level == meter - 1)
            {
                Draw(list, buffer);
                inserter.Invoke(array, buffer, indices);
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    indices[level] = i;
                    Distribute(list.Code[i].Move<EslonList>(), array, buffer, indices, level + 1);
                }
            }
        }

        private void Draw(EslonList list, T[] buffer)
        {
            int length = buffer.Length;

            for (int i = 0; i < length; i++)
            {
                buffer[i] = (T)element.Cast(list.Code[i], null);
            }
        }

        private int[] Measure(EslonList list)
        {
            int[] nx = new int[meter];

            int end = nx.Length - 1;

            for (int i = 0; i <= end && list.Count != 0; i++)
            {
                nx[i] = list.Count;

                if (i != end)
                {
                    list = list.Code[0].Move<EslonList>();
                }
            }

            return nx;
        }

        private EslonList Digest(Array array, T[] buffer, int[] indices, int level)
        {
            EslonList list = new EslonList(array.GetLength(level));

            bool final = (level == meter - 1);

            if (final)
            {
                extractor.Invoke(array, buffer, indices);
            }

            int length = array.GetLength(level);

            for (int i = 0; i < length; i++)
            {
                if (final)
                {
                    list.Add(element.Digest(buffer[i]));
                }
                else
                {
                    indices[level] = i;
                    list.Add(Digest(array, buffer, indices, level + 1));
                }
            }

            return list;
        }

        private delegate Array Caster(int[] format);
        private delegate void Copier(Array array, Array buffer, int[] indices);
    }
}
