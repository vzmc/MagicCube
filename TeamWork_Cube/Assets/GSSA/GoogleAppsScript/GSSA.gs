var Const = {
  Method : "$mt$",
  SheetName : "$sn$",
  Select: "$sl$",
  Distinct: "$di$",
  Where : "$wh$",
  ObjectId : "$oi$",
  Target : "$tg$",
  Value : "$vl$",
  Compare : "$cp$",
  OrderBy : "$ob$",
  Limit : "$li$",
  Skip : "$sk$",
  IsDesc : "$id$",
};

function doPost(e) {
  var res = e.parameter;

  var sheetName = res[Const.SheetName];
  delete res[Const.SheetName];
  
  var ss = SpreadsheetApp.getActiveSpreadsheet();
  if (ss.getSheetByName(sheetName) == null) {
    ss.insertSheet(sheetName,0);
  } 
  var sheet = ss.getSheetByName(sheetName);  

  var func = res[Const.Method];
  delete res[Const.Method];
  
  switch(func.toUpperCase()){
    case "SAVE":
      var lock = LockService.getScriptLock();
      var retData = {};
      try {
        lock.waitLock(1000); // 1�b�̃��b�N�J���҂�
        retData = SaveFunc(sheet,res);
      } catch (e) {
        retData[Const.ObjectId] = -1;
      } finally{
        lock.releaseLock();
        var retJson = JSON.stringify(retData,null,2);
        return ContentService.createTextOutput(retJson).setMimeType(ContentService.MimeType.JSON);
      }
      break;
    case "FIND":
      var data = FindFunc(sheet,res);
      var retJson = JSON.stringify(data,null,2);
      return ContentService.createTextOutput(retJson).setMimeType(ContentService.MimeType.JSON);
      break;
    case "COUNT":
      var data = FindFunc(sheet,res);
      var retJson = JSON.stringify({Count:data.values.length},null,2);
      return ContentService.createTextOutput(retJson).setMimeType(ContentService.MimeType.JSON);
      break;
  }
}

function SaveFunc(sheet,res){
  var postObjectId = res[Const.ObjectId];
  delete res[Const.ObjectId];
  
  //���ݎ��Ԃ����Ă���
  res["createTime"] = new Date().getTime().toString();
  
  var range = sheet.getDataRange();
  var sheetData = range.getValues();
  var headers = sheetData.splice(0, 1)[0];
  if(range.isBlank()){
    sheetData = [];
    headers = [];
  }
  var insertData = [];
  for(var d in res){
    var index = headers.indexOf(d);
    if(index >= 0){
      insertData[index] = res[d];
    }else{
       index = headers.push(d);
       insertData[index-1] = res[d];
    }
  }

  //�f�[�^�����̍X�V postObjectId�͍s�ԍ��Ɠ��ӁB�@sheetData.push�����l��0�I���W����sheet��̈ʒu�ɂ��邽��+1����K�v����B�炢
  var oid;
  if(postObjectId > 0){
    oid = postObjectId;
  }else{
    oid = sheetData.push(insertData) + 1;  
  }
  sheet.getRange(oid,1,1,insertData.length).setValues([insertData]);
  //�w�b�_�[�����̍X�V
  sheet.getRange(1,1,1,headers.length).setValues([headers]);
  
  var retCode = {};
  retCode[Const.ObjectId] = oid;
  return retCode;
}

function FindFunc(sheet,res){
  //�\�����Ɏ���Ă���
  var orderBy = res[Const.OrderBy];
  var isDesc = res[Const.IsDesc];
  var skip = res[Const.Skip];
  var limit = res[Const.Limit];
  var where = res[Const.Where];
  var select = res[Const.Select];
  var distinct = res[Const.Distinct];

  var range = sheet.getDataRange();
  var sheetData = range.getValues();
  var headers = sheetData.splice(0,1)[0];
  if(range.isBlank()){
    return ContentService.createTextOutput(JSON.stringify({keys:[]})).setMimeType(ContentService.MimeType.JSON);
  }
 
  var retData = sheetData.map(function(row,rindex){
    var data = {value:row};
    data[Const.ObjectId] = rindex+2;//TODO Header�������̂�2�𑫂��Ă�����
    return data;
  });
  
  if(where){
    var wheres = JSON.parse(where);
    retData = retData.filter(function(row,rindex){
      for(var wi in wheres){
        var w = wheres[wi];
        var key = w[Const.Target];
        var value = w[Const.Value];
        var index = headers.indexOf(key);
        if(index < 0)return false;
        //�ړI�ɍ���Ȃ��f�[�^�͒e��
        
        var compare = w[Const.Compare];
        switch(compare){
          case "EQ":
            if(!(row.value[index] == value))return false;
            break;
          case "NE":
            if(!(row.value[index] != value))return false;
            break;
          case "LT":
            if(!(row.value[index] < value))return false;
            break;
          case "GT":
            if(!(row.value[index] > value))return false;
            break;
          case "LE":
            if(!(row.value[index] <= value))return false;
            break;
          case "GE":
            if(!(row.value[index] >= value))return false;
            break;
        }
      }
      //���ł͂�����Ȃ������f�[�^�����߂Ă��钊�o�f�[�^
      return true;
    });
  }
  
  if(orderBy){
    var index = headers.indexOf(orderBy);
    if(index >= 0){
      retData = retData.sort(function(a,b){
        if(a.value[index] > b.value[index]) return 1 * isDesc;
        if(a.value[index] < b.value[index]) return -1 * isDesc;
        return 0;
      });
    }
  }
  
  //�d���𔲂�
  if(distinct){
    var index = headers.indexOf(distinct);
    if(index >= 0){
      retData = retData.filter(function (x, i, self) {
        var distinctTarget = x.value[index];       
        for(var findIndex = 0;findIndex < i;findIndex++){
          if(self[findIndex].value[index] == distinctTarget)return false;
        }
        return true;
      });
    }
  }
  
  if(skip)retData = retData.slice(skip);
  if(limit)retData = retData.slice(0,limit);
  
  //�ԋp�f�[�^��S�f�[�^�ł͂Ȃ��Aselect�Ŏw�肳��Ă��鍀�ڂ����ɂ��Ă�����
  if(select)
  {
    var selects = JSON.parse(select);

    for(var headerIndex = 0;headerIndex < headers.length;){
      var key = headers[headerIndex];
      var findIndex = selects.indexOf(key);
      if(findIndex < 0){
        for(var d in retData)
        {
          retData[d].value.splice(headerIndex,1);
        }
        headers.splice(headerIndex, 1); // index�̂Ƃ�����폜
        continue;
      }
      headerIndex++;
    }   
  }

  return {values:retData,keys:headers}; 
}
