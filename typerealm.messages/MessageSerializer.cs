namespace TypeRealm.Messages
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using ProtoBuf;

    public static class MessageSerializer
    {
        private static readonly Type[] _messageTypes = new []
        {
            // Sent to server.
            typeof(AttackCommand),
            typeof(ArriveToZoneCommand),
            typeof(Authorize),
            typeof(BattleMessage),
            typeof(FinishWalkingCommand),
            typeof(MoveForCommand),
            typeof(TurnAroundCommand),
            typeof(StartBattleCommand),
            typeof(StartMovingToZoneCommand),
            typeof(StopBattleCommand),
            typeof(ZoneMessage),

            // Sent to client.
            typeof(Status),
            typeof(Notification)
        };

        private static readonly Dictionary<int, Type> _indexToType = _messageTypes.Select(
            (x, i) => new { Index = i + 1, Message = x }).ToDictionary(x => x.Index, x => x.Message);
        private static readonly Dictionary<Type, int> _typeToIndex
            = _indexToType.ToDictionary(x => x.Value, x => x.Key);

        public static object Deserialize(Stream stream)
        {
            object message;

            if (Serializer.NonGeneric.TryDeserializeWithLengthPrefix(stream, PrefixStyle.Base128, fieldNumber => GetType(fieldNumber), out message))
                return message;

            throw new InvalidOperationException("Invalid message.");
        }

        public static void Serialize(Stream stream, object message)
        {
            var fieldNumber = GetFieldNumber(message.GetType());

            Serializer.NonGeneric.SerializeWithLengthPrefix(stream, message, PrefixStyle.Base128, fieldNumber);
        }

        private static Type GetType(int fieldNumber)
        {
            if (!_indexToType.ContainsKey(fieldNumber))
                throw new InvalidOperationException("Unknown field number.");

            return _indexToType[fieldNumber];
        }

        private static int GetFieldNumber(Type type)
        {
            if (!_typeToIndex.ContainsKey(type))
                throw new InvalidOperationException("Unknown message type.");

            return _typeToIndex[type];
        }
    }
}
