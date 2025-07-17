import pyodbc
from PIL import Image
import io

# 数据库连接信息
server = r'(localdb)\local_SQL'
# 或
# server = '(localdb)\\local_SQL'
database = 'NBA_players'
username = ''
password = ''

def show_player_photo(player_id):
    conn_str = f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={server};DATABASE={database};UID={username};PWD={password};CHARSET=UTF8'
    conn = pyodbc.connect(conn_str)
    cursor = conn.cursor()
    cursor.execute("SELECT photo FROM player_mapping WHERE player_id = ?", player_id)
    row = cursor.fetchone()
    conn.close()
    if row and row[0]:
        img_bytes = row[0]
        img = Image.open(io.BytesIO(img_bytes))
        img.show()
    else:
        print("未找到该球员的图片。")

if __name__ == "__main__":
    player_id = input("请输入球员ID：")
    try:
        player_id = int(player_id)
        show_player_photo(player_id)
    except ValueError:
        print("请输入有效的数字ID。")
