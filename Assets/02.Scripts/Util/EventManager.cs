using UnityEngine.Events;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EventLibrary
{
    // 제네릭 UnityEvent 클래스 정의
    [Serializable]
    public class GenericEvent<T> : UnityEvent<T> { }

    public class EventManager<E> where E : Enum
    {
        // 이벤트 이름과 해당 UnityEvent를 저장하는 딕셔너리
        private static readonly Dictionary<E, UnityEventBase> eventDictionary = new Dictionary<E, UnityEventBase>();
        // 스레드 안전성을 위한 객체
        private static readonly object lockObj = new object();

        // 매개변수가 없는 UnityAction 리스너를 추가하는 메서드
        public static void StartListening(E eventName, UnityAction listener)
        {
            lock (lockObj)
            {
                UnityEvent unityEvent = GetOrCreateEvent<UnityEvent>(eventName);
                unityEvent.AddListener(listener);
                Debug.Log($"Listener added to event: {eventName}");
            }
        }

        // 제네릭 매개변수를 갖는 UnityAction 리스너를 추가하는 메서드
        public static void StartListening<T>(E eventName, UnityAction<T> listener)
        {
            lock (lockObj)
            {
                GenericEvent<T> genericEvent = GetOrCreateEvent<GenericEvent<T>>(eventName);
                genericEvent.AddListener(listener);
                Debug.Log($"Listener added to event: {eventName}");
            }
        }


        // 매개변수가 없는 UnityAction 리스너를 제거하는 메서드
        public static void StopListening(E eventName, UnityAction listener)
        {
            lock (lockObj)
            {
                if (eventDictionary.TryGetValue(eventName, out var thisEvent) && thisEvent is UnityEvent unityEvent)
                {
                    unityEvent.RemoveListener(listener);
                    Debug.Log($"Listener removed from event: {eventName}");
                    RemoveEventIfEmpty(eventName, unityEvent);
                }
            }
        }

        // 제네릭 매개변수를 갖는 UnityAction 리스너를 제거하는 메서드
        public static void StopListening<T>(E eventName, UnityAction<T> listener)
        {
            lock (lockObj)
            {
                if (eventDictionary.TryGetValue(eventName, out var thisEvent) && thisEvent is GenericEvent<T> genericEvent)
                {
                    genericEvent.RemoveListener(listener);
                    Debug.Log($"Listener removed from event: {eventName}");
                    RemoveEventIfEmpty(eventName, genericEvent);
                }
            }
        }

        // 매개변수가 없는 이벤트를 트리거하는 메서드
        public static void TriggerEvent(E eventName)
        {
            lock (lockObj)
            {
                try
                {
                    if (eventDictionary.TryGetValue(eventName, out var thisEvent) && thisEvent is UnityEvent unityEvent)
                    {
                        unityEvent.Invoke();
                        Debug.Log($"Event triggered: {eventName}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error triggering event {eventName}: {e.Message}");
                }
            }
        }

        // 제네릭 매개변수를 갖는 이벤트를 트리거하는 메서드
        public static void TriggerEvent<T>(E eventName, T parameter)
        {
            lock (lockObj)
            {
                try
                {
                    if (eventDictionary.TryGetValue(eventName, out var thisEvent) && thisEvent is GenericEvent<T> genericEvent)
                    {
                        genericEvent.Invoke(parameter);
                        Debug.Log($"Event triggered: {eventName} with parameter: {parameter}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error triggering event {eventName} with parameter {parameter}: {e.Message}");
                }
            }
        }

        // 이벤트가 존재하지 않으면 생성하여 반환하는 메서드
        private static TEvent GetOrCreateEvent<TEvent>(E eventName) where TEvent : UnityEventBase, new()
        {
            if (!eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent = new TEvent();
                eventDictionary.Add(eventName, thisEvent);
                Debug.Log($"Event created: {eventName}");
            }
            return thisEvent as TEvent;
        }

        // 이벤트가 비어 있으면 딕셔너리에서 제거하는 메서드
        private static void RemoveEventIfEmpty(E eventName, UnityEventBase thisEvent)
        {
            if (thisEvent.GetPersistentEventCount() == 0)
            {
                eventDictionary.Remove(eventName);
                Debug.Log($"Event removed: {eventName}");
            }
        }
    }
}
