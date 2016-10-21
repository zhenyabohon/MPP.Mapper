using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MPP.Mapper
{
    public class Mapper : IMapper
    {
        public Mapper()
        {
            ImplicitNumericConversionsTable = GetImplicitNumericConversionsTable();
            CachedExpressions = new Dictionary<Tuple<Type, Type>, LambdaExpression>();
        }

        private Dictionary<Type, IEnumerable<Type>> ImplicitNumericConversionsTable { get; }

        private Dictionary<Tuple<Type, Type>, LambdaExpression> CachedExpressions { get; }


        public TDestination Map<TSource, TDestination>(TSource source) where TDestination : new()
        {
            var dest = CreateMapFunc<TSource, TDestination>().Invoke(source);

            return dest;
        }

        public Func<TInput, TOutput> CreateMapFunc<TInput, TOutput>()
        {
            var source = Expression.Parameter(typeof(TInput), "source");
            var destination = Expression.Parameter(typeof(TOutput), "destination");

            if (CachedExpressions.ContainsKey(new Tuple<Type, Type>(typeof(TInput), typeof(TOutput))))
            {
                var d = CachedExpressions[new Tuple<Type, Type>(typeof(TInput), typeof(TOutput))].Compile();
                Func<TInput, TOutput> func = x => (TOutput) d.DynamicInvoke(x);

                return func;
            }

            var body = Expression.MemberInit(Expression.New(typeof(TOutput)),
                source.Type.GetProperties()
                    .Where(p =>
                    {
                        var hasDestinationPropertiesWithSameName = destination.Type.GetProperties()
                            .FirstOrDefault(x => x.Name == p.Name);
                        var destinationPropertyWithSameName =
                            destination.Type.GetProperties().FirstOrDefault(x => x.Name == p.Name);
                        return (destinationPropertyWithSameName != null)
                               && (hasDestinationPropertiesWithSameName != null)
                               && destinationPropertyWithSameName.CanWrite &&
                               destination.Type.GetProperties().Select(x => x.Name).Contains(p.Name) &&
                               IsBaseTypeCanBeConvert(p.PropertyType,
                                   hasDestinationPropertiesWithSameName
                                       .PropertyType);
                    }).Select(p =>
                        Expression.Bind(typeof(TOutput).GetProperty(p.Name),
                            Expression.Convert(
                                Expression.Property(source, p), typeof(TOutput).GetProperty(p.Name).PropertyType))));
            var expr = Expression.Lambda<Func<TInput, TOutput>>(body, source);
            CachedExpressions.Add(new Tuple<Type, Type>(typeof(TInput), typeof(TOutput)), expr);
            return expr.Compile();
        }

        private bool IsBaseTypeCanBeConvert(Type source, Type destination)
        {
            if (source == destination)
                return true;

            return ImplicitNumericConversionsTable.ContainsKey(source) &&
                   ImplicitNumericConversionsTable[source].Contains(destination);
        }


        private Dictionary<Type, IEnumerable<Type>> GetImplicitNumericConversionsTable()
        {
            var result = new Dictionary<Type, IEnumerable<Type>>();
            result.Add(typeof(sbyte),
                new List<Type>
                {
                    typeof(short),
                    typeof(int),
                    typeof(long),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                });
            result.Add(typeof(byte),
                new List<Type>
                {
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                });
            result.Add(typeof(short),
                new List<Type> {typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal)});
            result.Add(typeof(ushort),
                new List<Type>
                {
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                });
            result.Add(typeof(int), new List<Type> {typeof(long), typeof(float), typeof(double), typeof(decimal)});
            result.Add(typeof(uint),
                new List<Type> {typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal)});
            result.Add(typeof(long), new List<Type> {typeof(float), typeof(double), typeof(decimal)});
            result.Add(typeof(float), new List<Type> {typeof(double)});
            result.Add(typeof(ulong), new List<Type> {typeof(float), typeof(double), typeof(decimal)});

            return result;
        }
    }
}