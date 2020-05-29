using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Linq.Expressions;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace HTLib2
{
    public static partial class HObjectDynamic
    {
        // https://stackoverflow.com/questions/8631546/get-property-value-from-c-sharp-dynamic-object-by-string-reflection/14011692
        public static string GetPropertyString(dynamic o, string member) { return (string)GetProperty(o, member);  }
        public static int    GetPropertyInt   (dynamic o, string member) { return (int   )GetProperty(o, member);  }
        public static double GetPropertyDouble(dynamic o, string member) { return (double)GetProperty(o, member);  }
        public static object GetProperty(dynamic o, string member)
        {
            if(o == null) throw new ArgumentNullException("o");
            if(member == null) throw new ArgumentNullException("member");
            Type scope = o.GetType();
            IDynamicMetaObjectProvider provider = o as IDynamicMetaObjectProvider;
            if(provider != null)
            {
                ParameterExpression param = Expression.Parameter(typeof(object));
                DynamicMetaObject mobj = provider.GetMetaObject(param);
                GetMemberBinder binder = (GetMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, member, scope, new CSharpArgumentInfo[]{CSharpArgumentInfo.Create(0, null)});
                DynamicMetaObject ret = mobj.BindGetMember(binder);
                BlockExpression final = Expression.Block(
                    Expression.Label(CallSiteBinder.UpdateLabel),
                    ret.Expression
                );
                LambdaExpression lambda = Expression.Lambda(final, param);
                Delegate del = lambda.Compile();
                return del.DynamicInvoke(o);
            }else{
                return o.GetType().GetProperty(member, BindingFlags.Public | BindingFlags.Instance).GetValue(o, null);
            }
        }
    }
}
