using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GSSA
{
    /// <summary>
    /// GoogleSpreadSheet�̃f�[�^�i1�s�j��\���f�[�^�I�u�W�F�N�g
    /// </summary>
    public class SpreadSheetObject : Dictionary<string, object>
    {
        private string sheetName;
        private int objectId = -1; //ID�Ƃ������A�s�ԍ�
        public static string Id { get { return SystemInfo.deviceUniqueIdentifier; } }

        /// <summary>
        /// �ێ����Ă���f�[�^��int�ɂ��ĕԋp�B
        /// ��U������ɂ��Ă���parse����̂Ŏ኱�x��
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int ParseInt(string key)
        {
            return int.Parse(this[key].ToString());
        }
        /// <summary>
        /// �ێ����Ă���f�[�^��float�ɂ��ĕԋp�B
        /// ��U������ɂ��Ă���parse����̂Ŏ኱�x��
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float ParseFloat(string key)
        {
            return float.Parse(this[key].ToString());
        }

        /// <summary>
        /// �ێ����Ă���f�[�^��string�ɂ��ĕԋp
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ParseString(string key)
        {
            return this[key].ToString();
        }

        /// <summary>
        /// �R���X�g���N�^
        /// sheetName���ȗ�(null)�ɂ����ꍇ�́ASpreadSheetSetting��DefalutSheetName���g�p
        /// objectId�͊�{�w�肵�Ȃ����A�����Ďw�肷�邱�ƂŁASpreadSheet�̍s�����w�肵�ăf�[�^��ێ��ł���
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="objectId"></param>
        public SpreadSheetObject(string sheetName = null, int objectId = -1)
        {
            this.sheetName = sheetName ?? SpreadSheetSetting.Instance.DefalutSheetName;
            this.objectId = objectId;
        }

        /// <summary>
        /// �ۑ�����
        /// Coroutine�̒��ł����yield return�őҋ@�\
        /// �ԋp�l(int)�͂��̂܂�objectId�Ƃ��Ċi�[�����B�@�����̏ꍇ�͕ۑ��������s�B
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public CustomYieldInstruction SaveAsync(Action<int> callback = null)
        {
            var complete = false;
            SpreadSheetSetting.Instance.Enqueue(()=>SaveAsyncIterator(callback,b => complete = b));
            return new WaitUntil(() => complete);
        }

        private IEnumerator SaveAsyncIterator(Action<int> callback,Action<bool> complete)
        {
            var form = new WWWForm();
            form.AddField(SpreadSheetConst.Method, "Save");
            form.AddField(SpreadSheetConst.SheetName, sheetName);
            form.AddField(SpreadSheetConst.ObjectId, objectId);
            foreach (var pair in this)
            {
                form.AddField(pair.Key, pair.Value.ToString());
            }

            using (var www = UnityWebRequest.Post(SpreadSheetSetting.Instance.SpreadSheetUrl, form))
            {
                yield return www.Send();
                if (SpreadSheetSetting.Instance.IsDebugLogOutput)
                {
                    Debug.Log("GSSA SaveAsync Response:\n" + www.downloadHandler.text);
                }
                var jsonNode = JsonNode.Parse(www.downloadHandler.text);
                objectId = jsonNode[SpreadSheetConst.ObjectId].GetInt();
                if(callback != null)callback.Invoke(objectId);
            }
            complete(true);
        }
    }
}
