import requests

try:
  url = 'https://bit.ly/lidwizard'
  session = requests.Session()
  response = session.head(
    url, allow_redirects=True, timeout=10)
  filename = 'lidwizard.xml'
  response = session.get(
    response.url, stream=True, timeout=10)
  response.raise_for_status()
  with open(filename, 'wb') as file:
    for chunk in response.iter_content(
      chunk_size=8192):
      file.write(chunk)
  print("Успешно")
except \
  requests.exceptions.RequestException \
  as e:
  print(f"Ошибка: {e}")
except Exception as e:
  print(f"Неожиданная ошибка: {e}")
