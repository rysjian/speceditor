import requests
import os
from urllib.parse import urlparse

def download_xml_file(url, custom_filename=None):
    """
    Скачивает XML-файл по указанной ссылке (включая короткие ссылки bit.ly)
    и сохраняет его с указанным именем или автоматически определяемым именем.
    
    :param url: Ссылка на XML-файл (может быть короткой ссылкой)
    :param custom_filename: Желаемое имя файла (если None, имя определится автоматически)
    :return: Путь к сохраненному файлу или None в случае ошибки
    """
    try:
        # Разрешаем короткие ссылки (получаем конечный URL)
        session = requests.Session()
        response = session.head(url, allow_redirects=True, timeout=10)
        final_url = response.url
        
        # Получаем имя файла из URL или используем custom_filename
        if custom_filename:
            filename = custom_filename
            if not filename.endswith('.xml'):
                filename += '.xml'
        else:
            parsed_url = urlparse(final_url)
            filename = os.path.basename(parsed_url.path)
            if not filename:
                filename = 'lidwizard.xml'
            elif not filename.endswith('.xml'):
                filename += '.xml'

        # Получаем содержимое файла
        response = session.get(final_url, stream=True, timeout=10)
        response.raise_for_status()

        # Создаем папку для загрузок, если ее нет
        download_dir = os.path.join(os.getcwd(), 'downloads')
        os.makedirs(download_dir, exist_ok=True)
        
        # Полный путь к файлу
        save_path = os.path.join(download_dir, filename)

        # Сохраняем файл
        with open(save_path, 'wb') as file:
            for chunk in response.iter_content(chunk_size=8192):
                file.write(chunk)

        print(f"Файл успешно скачан и сохранен как: {save_path}")
        return save_path

    except requests.exceptions.RequestException as e:
        print(f"Ошибка при скачивании файла: {e}")
        return None
    except Exception as e:
        print(f"Неожиданная ошибка: {e}")
        return None

# Пример использования
if __name__ == "__main__":
    # Ссылка для скачивания
    short_url = "https://bit.ly/lidwizard"
    
    # Вариант 1: Скачать с автоматическим именем
    # downloaded_file = download_xml_file(short_url)
    
    # Вариант 2: Скачать с собственным именем
    downloaded_file = download_xml_file(short_url, custom_filename="my_custom_name.xml")
    
    if downloaded_file:
        print(f"XML-файл успешно сохранен: {downloaded_file}")
    else:
        print("Не удалось скачать XML-файл")