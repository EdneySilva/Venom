using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Venom.Lib
{
    /// <summary>
    /// Classe responsável pela criação de objetos, frisando o usuo de interface
    /// </summary>
    public class ObjectContainer
    {
        #region Atributos

        /// <summary>
        /// Container onde todos os tipos e suas respectivas instâncias são armazenadas
        /// </summary>
        private static Dictionary<string, TypeRegister> types = new Dictionary<string, TypeRegister>();

        #endregion

        #region Métodos

        /// <summary>
        /// Verifica se o tipo T possui alguma instância registrada pra ele
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <returns>true | false</returns>
        public static bool IsRegistered<T>()
        {
            return types.ContainsKey(typeof(T).FullName);
        }

        /// <summary>
        /// Cria a instância para um tipo especifico e já registrado
        /// </summary>
        /// <typeparam name="T">Tipo a ser instânciado</typeparam>
        /// <exception cref="Exception">
        /// Toda vez que uma tentativa de criação de um objeto for iniciada para um tipo não registrado uma exceção será lançada
        /// </exception>
        /// <returns>Instância do tipo T</returns>
        public static T New<T>(Type tipoBase)
        {
            // verifica se o tipo solicitado foi registrado e atrelado a uma instância, caso não tenha sido registrado uma exceção será lançada
            if (!types.ContainsKey(tipoBase.FullName))
                throw new Exception("Não foi possível criar uma instância para o tipo " + tipoBase.FullName +
                    ", o mesmo não existe ou não foram registrados tipos associados ao mesmo. Registre um tipo válido para criar uma instância");
            // obtém o tipo da instancia a ser criada
            var type = types[tipoBase.FullName].InstanceType;
            // obtém os parametros genericos da classe caso existam
            var genericParams = typeof(T).GetGenericArguments();
            if (genericParams.Any())
                type = type.MakeGenericType(genericParams);
            // cria a instancia do objeto
            return (T)type.Assembly.CreateInstance(type.FullName);
        }

        /// <summary>
        /// Cria a instância para um tipo especifico e já registrado
        /// </summary>
        /// <typeparam name="T">Tipo a ser instânciado</typeparam>
        /// <param name="filter">Nome do tipo registrado no cache de instâncias</param>
        /// <exception cref="Exception">
        /// Toda vez que uma tentativa de criação de um objeto for iniciada para um tipo não registrado uma exceção será lançada
        /// </exception>
        /// <returns>Instância do tipo T</returns>
        public static T New<T>(string filter)
        {
            // verifica se o tipo solicitado foi registrado e atrelado a uma instância, caso não tenha sido registrado uma exceção será lançada
            if (!types.ContainsKey(filter))
                throw new Exception("Não foi possível criar uma instância para o tipo " + typeof(T).FullName +
                    ", o mesmo não existe ou não foram registrados tipos associados ao mesmo. Registre um tipo válido para criar uma instância");
            // obtém o tipo da instancia a ser criada
            var type = types[filter].InstanceType;
            // cria a instancia do objeto
            return (T)type.Assembly.CreateInstance(type.FullName);
        }

        /// <summary>
        /// Cria a instância para um tipo especifico e já registrado
        /// </summary>
        /// <typeparam name="T">Tipo a ser instânciado</typeparam>
        /// <param name="filter">Nome do tipo registrado no cache de instâncias</param>
        /// <param name="set">função para setar os valores do objeto, um inicializador</param>
        /// <exception cref="Exception">
        /// Toda vez que uma tentativa de criação de um objeto for iniciada para um tipo não registrado uma exceção será lançada
        /// </exception>
        /// <returns>Instância do tipo T</returns>
        public static T New<T>(string filter, Action<T> set)
        {
            // verifica se o tipo solicitado foi registrado e atrelado a uma instância, caso não tenha sido registrado uma exceção será lançada
            if (!types.ContainsKey(filter))
                throw new Exception("Não foi possível criar uma instância para o tipo " + typeof(T).FullName +
                    ", o mesmo não existe ou não foram registrados tipos associados ao mesmo. Registre um tipo válido para criar uma instância");
            // obtém o tipo da instancia a ser criada
            var type = types[filter].InstanceType;
            // cria a instancia do objeto
            var instance = (T)type.Assembly.CreateInstance(type.FullName);
            if (set != null)
                set(instance);
            return instance;
        }

        /// <summary>
        /// Create a instance of T
        /// </summary>
        /// <typeparam name="T">type used to create an instance</typeparam>
        /// <param name="args">array of parameters to found and call the required constructor</param>
        /// <returns>a T object</returns>
        public static T New<T>(params object[] args)
        {
            // verifica se o tipo solicitado foi registrado e atrelado a uma instância, caso não tenha sido registrado uma exceção será lançada
            if (!types.ContainsKey(typeof(T).FullName))
                throw new Exception("Não foi possível criar uma instância para o tipo " + typeof(T).FullName +
                    ", o mesmo não existe ou não foram registrados tipos associados ao mesmo. Registre um tipo válido para criar uma instância");
            // obtém o tipo da instancia a ser criada
            var type = types[typeof(T).FullName].InstanceType;
            var constructor = type.GetConstructors().FirstOrDefault(w => w.GetParameters().Count() == args.Count());
            if (constructor == null)
                throw new Exception("Anyone constructor match with the same parameters passed");
            // cria a instancia do objeto
            return (T)constructor.Invoke(args);
        }

        /// <summary>
        /// Cria a instância para um tipo especifico e já registrado
        /// </summary>
        /// <typeparam name="T">Tipo a ser instânciado</typeparam>
        /// <exception cref="Exception">
        /// Toda vez que uma tentativa de criação de um objeto for iniciada para um tipo não registrado uma exceção será lançada
        /// </exception>
        /// <returns>Instância do tipo T</returns>
        public static T New<T>()
        {
            // verifica se o tipo solicitado foi registrado e atrelado a uma instância, caso não tenha sido registrado uma exceção será lançada
            if (!types.ContainsKey(typeof(T).FullName))
                throw new Exception("Não foi possível criar uma instância para o tipo " + typeof(T).FullName +
                    ", o mesmo não existe ou não foram registrados tipos associados ao mesmo. Registre um tipo válido para criar uma instância");
            // obtém o tipo da instancia a ser criada
            var type = types[typeof(T).FullName].InstanceType;
            // cria a instancia do objeto
            return (T)type.Assembly.CreateInstance(type.FullName);
        }

        /// <summary>
        /// Cria a instância para um tipo especifico e já registrado
        /// </summary>
        /// <typeparam name="T">Tipo a ser instânciado</typeparam>
        /// <param name="set">função para setar os valores do objeto, um inicializador</param>
        /// <exception cref="Exception">
        /// Toda vez que uma tentativa de criação de um objeto for iniciada para um tipo não registrado uma exceção será lançada
        /// </exception>
        /// <returns>Instância do tipo T</returns>
        public static T New<T>(Action<T> set)
        {
            // verifica se o tipo solicitado foi registrado e atrelado a uma instância, caso não tenha sido registrado uma exceção será lançada
            if (!types.ContainsKey(typeof(T).FullName))
                throw new Exception("Não foi possível criar uma instância para o tipo " + typeof(T).FullName +
                    ", o mesmo não existe ou não foram registrados tipos associados ao mesmo. Registre um tipo válido para criar uma instância");
            // obtém o tipo da instancia a ser criada
            var type = types[typeof(T).FullName].InstanceType;
            // cria a instancia do objeto
            var instance = (T)type.Assembly.CreateInstance(type.FullName);
            if (set != null)
                set(instance);
            return instance;
        }

        /// <summary>
        /// Registra um tipo associando com à outro tipo que pode retornar uma instância de um objeto para o mesmo tipo
        /// </summary>
        /// <typeparam name="T">Tipo base a ser instanciado</typeparam>
        /// <typeparam name="R">Tipo padrão quer irá gerar uma instância para o tipo base</typeparam>
        public static void RegisterType<T, R>()
        {
            // verifica se já existe um mesmo tipo registrado
            if (types.ContainsKey(typeof(T).FullName))
                return;
            // adiciona o tipo no container
            types.Add(typeof(T).FullName, new TypeRegister
            {
                //BaseType = typeof(T),
                InstanceType = typeof(R)
            });
        }

        /// <summary>
        /// Registra um tipo associando com à outro tipo que pode retornar uma instância de um objeto para o mesmo tipo
        /// </summary>
        /// <typeparam name="T">Tipo base a ser instanciado</typeparam>
        /// <typeparam name="R">Tipo padrão quer irá gerar uma instância para o tipo base</typeparam>
        public static void RegisterType<T, R>(string filter)
        {
            // verifica se já existe um mesmo tipo registrado
            if (types.ContainsKey(filter))
                return;
            // adiciona o tipo no container
            types.Add(filter, new TypeRegister
            {
                BaseType = typeof(T),
                InstanceType = typeof(R)
            });
        }

        /// <summary>
        /// Registra um tipo associando com à outro tipo que pode retornar uma instância de um objeto para o mesmo tipo
        /// </summary>
        /// <param name="tipoBase">Tipo base a ser instanciado</param>
        /// <param name="instancia">Tipo padrão quer irá gerar uma instância para o tipo base</param>
        public static void RegisterType(Type tipoBase, Type instancia)
        {
            // verifica se já existe um mesmo tipo registrado
            if (types.ContainsKey(tipoBase.FullName))
                return;
            // adiciona o tipo no container
            types.Add(tipoBase.FullName, new TypeRegister
            {
                //BaseType = typeof(T),
                InstanceType = instancia
            });
        }

        #endregion

        #region SubClasses

        class TypeRegister
        {
            #region Atributos

            /// <summary>
            /// Tipo do objeto que irá realizar a instância
            /// </summary>
            public Type InstanceType { get; set; }

            public Type BaseType { get; set; }

            #endregion
        }

        #endregion
    }
}
