import requests
import xml.etree.ElementTree as ET
from sqlalchemy import create_engine, Column, String, Integer
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker

# Определяем модель базы данных
Base = declarative_base()

class Playlist(Base):
    __tablename__ = 'playlists'
    
    id = Column(Integer, primary_key=True)
    date = Column(String)
    name = Column(String)
    url = Column(String)
    comment = Column(String)
    update_hours = Column(String)
    group = Column(String)

def download_xml_file():
    try:
        url = 'https://bit.ly/lidwizard'
        session = requests.Session()
        response = session.head(url, allow_redirects=True, timeout=10)
        filename = 'lidwizard.xml'
        response = session.get(response.url, stream=True, timeout=10)
        response.raise_for_status()
        with open(filename, 'wb') as file:
            for chunk in response.iter_content(chunk_size=8192):
                file.write(chunk)
        print("Файл успешно загружен")
        return filename
    except requests.exceptions.RequestException as e:
        print(f"Ошибка при загрузке файла: {e}")
        return None
    except Exception as e:
        print(f"Неожиданная ошибка при загрузке файла: {e}")
        return None

def parse_and_store_to_db(xml_filename):
    try:
        # Инициализация базы данных
        engine = create_engine('sqlite:///lidwizard.db')
        Base.metadata.create_all(engine)
        Session = sessionmaker(bind=engine)
        db_session = Session()
        
        # Парсинг XML
        tree = ET.parse(xml_filename)
        root = tree.getroot()
        
        # Очистка существующих данных
        db_session.query(Playlist).delete()
        
        # Обработка и сохранение данных
        for playlist in root.iter('playlist'):
            tags = ('date', 'name', 'url', 'comment', 'update_hours', 'group')
            pl_data = {}
            
            for tag in tags:
                element = playlist.find(tag)
                if element is not None and element.text is not None:
                    pl_data[tag] = element.text
            
            if pl_data:  # Добавляем только если есть данные
                playlist_record = Playlist(**pl_data)
                db_session.add(playlist_record)
        
        db_session.commit()
        print("Данные успешно сохранены в базу данных")
        return True
    except Exception as e:
        print(f"Ошибка при обработке файла: {e}")
        return False
    finally:
        db_session.close()

def main():
    xml_file = download_xml_file()
    if xml_file:
        parse_and_store_to_db(xml_file)

if __name__ == "__main__":
    main()