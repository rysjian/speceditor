import xml.etree.ElementTree as ET
import json

try:
  filename = 'lidwizard.xml'
  tree = ET.parse(filename)
  root = tree.getroot()
  filedata = {'playlists': []}
  for playlist in root.iter('playlist'):
    tags = ('date','name','url',\
      'comment','update_hours','group')
    pl = {}
    for tag in tags:
      find = playlist.find(tag)
      if find not in [{},None]:
        pl[tag] = find.text
    filedata['playlists'].append(pl)
  filename = 'lidwizard.json'
  with open(filename, 'w') as file:
    text = json.dumps(filedata)
    file.write(text)
except Exception as e:
  print(f"Ошибка: {e}")