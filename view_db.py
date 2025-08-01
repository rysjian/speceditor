import sqlite3

def view_first_five_simple():
    try:
        conn = sqlite3.connect('lidwizard.db')
        cursor = conn.cursor()
        
        cursor.execute("SELECT * FROM playlists LIMIT 5")
        records = cursor.fetchall()
        
        if not records:
            print("В базе данных нет записей.")
            return
        
        print("\nПервые 5 записей (простой вариант):")
        for row in records:
            print(row)
            
    except sqlite3.Error as e:
        print(f"Ошибка SQLite: {e}")
    finally:
        if conn:
            conn.close()

view_first_five_simple()