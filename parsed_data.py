import os
import xml.etree.ElementTree as ET
from pathlib import Path

# Пути к файлам
DOWNLOADS_DIR = os.path.join(os.getcwd(), 'downloads')
SPECEDITOR_FILE = os.path.join(DOWNLOADS_DIR, 'my_custom_name.xml')  # Или другое имя файла

def parse_speceditor_xml(xml_file):
    """
    Парсит XML-файл speceditor и извлекает полезные данные.
    Возвращает структурированные данные в виде словаря.
    """
    try:
        # Проверяем существование файла
        if not os.path.exists(xml_file):
            raise FileNotFoundError(f"XML файл не найден: {xml_file}")
        
        # Парсим XML
        tree = ET.parse(xml_file)
        root = tree.getroot()
        
        # Здесь нужно знать структуру XML для правильного парсинга
        # Приведу пример для гипотетической структуры:
        result = {
            'channels': [],
            'metadata': {}
        }
        
        # Пример парсинга каналов (адаптируйте под реальную структуру)
        for channel in root.findall('.//channel'):
            channel_data = {
                'id': channel.get('id'),
                'name': channel.find('name').text if channel.find('name') is not None else None,
                'url': channel.find('url').text if channel.find('url') is not None else None,
                'group': channel.get('group'),
                'logo': channel.find('logo').text if channel.find('logo') is not None else None
            }
            result['channels'].append(channel_data)
        
        # Пример парсинга метаданных
        metadata = root.find('metadata')
        if metadata is not None:
            result['metadata'] = {
                'version': metadata.get('version'),
                'date': metadata.find('date').text if metadata.find('date') is not None else None,
                'author': metadata.find('author').text if metadata.find('author') is not None else None
            }
        
        return result
    
    except ET.ParseError as e:
        print(f"Ошибка парсинга XML: {e}")
        return None
    except Exception as e:
        print(f"Ошибка при обработке файла: {e}")
        return None

def save_parsed_data(data, output_format='json'):
    """
    Сохраняет распарсенные данные в выбранном формате.
    Поддерживаемые форматы: json, txt
    """
    if not data:
        print("Нет данных для сохранения")
        return
    
    try:
        output_dir = os.path.join(os.getcwd(), 'parsed_results')
        os.makedirs(output_dir, exist_ok=True)
        
        if output_format == 'json':
            import json
            output_file = os.path.join(output_dir, 'parsed_data.json')
            with open(output_file, 'w', encoding='utf-8') as f:
                json.dump(data, f, ensure_ascii=False, indent=4)
            print(f"Данные сохранены в JSON: {output_file}")
        
        elif output_format == 'txt':
            output_file = os.path.join(output_dir, 'parsed_data.txt')
            with open(output_file, 'w', encoding='utf-8') as f:
                # Просто пример - адаптируйте под свои нужды
                f.write("Распарсенные данные:\n\n")
                f.write(f"Всего каналов: {len(data['channels'])}\n")
                f.write("Метаданные:\n")
                for key, value in data['metadata'].items():
                    f.write(f"{key}: {value}\n")
                f.write("\nКаналы:\n")
                for channel in data['channels']:
                    f.write(f"{channel['name']} ({channel['id']}): {channel['url']}\n")
            print(f"Данные сохранены в TXT: {output_file}")
        
        else:
            print(f"Неподдерживаемый формат: {output_format}")
    
    except Exception as e:
        print(f"Ошибка при сохранении данных: {e}")

if __name__ == "__main__":
    # Парсим файл
    parsed_data = parse_speceditor_xml(SPECEDITOR_FILE)
    
    if parsed_data:
        print(f"Успешно распарсено {len(parsed_data['channels'])} каналов")
        
        # Сохраняем в разных форматах
        save_parsed_data(parsed_data, 'json')
        save_parsed_data(parsed_data, 'txt')
    else:
        print("Не удалось распарсить XML файл")