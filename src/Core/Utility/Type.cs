namespace AlbedoTeam.Sdk.QueryLanguage.Core.Utility
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class Type<T>
    {
        public static MemberInfo Member<TOut>(Expression<Func<T, TOut>> memberSelector)
        {
            if (memberSelector == null)
                throw new ArgumentNullException(nameof(memberSelector));

            var exp = memberSelector.Body;

            while (exp is UnaryExpression expression)
                exp = expression.Operand;

            if (!(exp is MemberExpression memberExpression))
                throw new ArgumentException($"{nameof(memberSelector)} is a not a valid member");

            return memberExpression.Member;
        }

        public static MethodInfo Method<TOut>(Expression<Func<T, TOut>> methodSelector)
        {
            if (methodSelector == null)
                throw new ArgumentNullException(nameof(methodSelector));

            var exp = methodSelector.Body;

            while (exp is UnaryExpression expression)
                exp = expression.Operand;

            if (!(exp is MethodCallExpression methodExpression))
                throw new ArgumentException($"{nameof(methodSelector)} is a not a valid method");

            return methodExpression.Method;
        }
    }
}