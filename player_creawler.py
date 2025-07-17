from ast import For
from re import findall
from bs4 import BeautifulSoup
import requests
import re
import pyodbc

# 数据库连接信息
server = '(localdb)\local_SQL'
database = 'NBA_players'
username = ''
password = ''

def test_db_connection():
    """测试数据库连接"""
    try:
        conn_str = f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={server};DATABASE={database};UID={username};PWD={password};CHARSET=UTF8'
        conn = pyodbc.connect(conn_str)
        cursor = conn.cursor()
        print("数据库连接成功！")
        conn.close()
        return True
    except Exception as e:
        print(f"数据库连接失败: {e}")
        return False

def fetch_html(url):
    """获取网页内容"""
    headers = {
        "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36",
        "Referer": "https://nba.hupu.com/stats",
        "Accept-Language": "en-US,en;q=0.9",
        "Connection": "keep-alive"
    }
    try:
        response = requests.get(url, headers=headers, timeout=10)
        response.raise_for_status()
        response.encoding = response.apparent_encoding
        return response.text
    except requests.RequestException as e:
        print(f"请求失败: {e}")
        return None

def get_player_id(conn, player_name):
    """获取球员 ID，如果不存在则创建"""
    try:
        cursor = conn.cursor()
        # 查询球员是否存在
        sql = f"SELECT player_id, player_name FROM player_mapping WHERE player_name = ?"
        cursor.execute(sql, player_name)
        result = cursor.fetchone()
        if result:
            return result[0]
        else:
            # 插入新球员
            sql = "INSERT INTO player_mapping (player_name) VALUES (?)"
            cursor.execute(sql, player_name)
            conn.commit()
            # 获取新插入的 player_id
            cursor.execute("SELECT MAX(player_id) FROM player_mapping")
            player_id = cursor.fetchone()[0]
            print(f"新增球员 {player_name}, ID: {player_id}")
            return player_id
    except Exception as e:
        print(f"获取球员 ID 失败: {e}")
        conn.rollback()
        return None

def create_player_table(conn, player_id):
    """创建球员数据表"""
    table_name = f"player_{player_id}"
    try:
        cursor = conn.cursor()
        sql = f"""
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '{table_name}')
        BEGIN
            CREATE TABLE {table_name} (
                日期 VARCHAR(20) COLLATE Chinese_PRC_CI_AS,
                对手 VARCHAR(50) COLLATE Chinese_PRC_CI_AS,
                比分 VARCHAR(20) COLLATE Chinese_PRC_CI_AS,
                时间 VARCHAR(10) COLLATE Chinese_PRC_CI_AS,
                投篮 VARCHAR(10) COLLATE Chinese_PRC_CI_AS,
                命中率 VARCHAR(10) COLLATE Chinese_PRC_CI_AS,
                三分 VARCHAR(10) COLLATE Chinese_PRC_CI_AS,
                三分命中率 VARCHAR(10) COLLATE Chinese_PRC_CI_AS,
                罚球 VARCHAR(10) COLLATE Chinese_PRC_CI_AS,
                罚球命中率 VARCHAR(10) COLLATE Chinese_PRC_CI_AS,
                篮板 VARCHAR(10) COLLATE Chinese_PRC_CI_AS,
                助攻 VARCHAR(10) COLLATE Chinese_PRC_CI_AS,
                抢断 VARCHAR(10) COLLATE Chinese_PRC_CI_AS,
                盖帽 VARCHAR(10) COLLATE Chinese_PRC_CI_AS,
                失误 VARCHAR(10) COLLATE Chinese_PRC_CI_AS,
                犯规 VARCHAR(10) COLLATE Chinese_PRC_CI_AS,
                得分 VARCHAR(10) COLLATE Chinese_PRC_CI_AS
            )
        END
        """
        cursor.execute(sql)
        conn.commit()
        print(f"表 {table_name} 创建成功")
    except Exception as e:
        print(f"表 {table_name} 创建失败: {e}")
        conn.rollback()

def table_exists(conn, table_name):
    """判断表是否存在"""
    cursor = conn.cursor()
    sql = "SELECT COUNT(*) FROM sys.tables WHERE name = ?"
    cursor.execute(sql, table_name)
    return cursor.fetchone()[0] > 0

def insert_player_data(conn, player_id, data):
    """插入球员数据"""
    table_name = f"player_{player_id}"
    try:
        cursor = conn.cursor()
        sql = f"""
        INSERT INTO {table_name} (日期, 对手, 比分, 时间, 投篮, 命中率, 三分, 三分命中率, 罚球, 罚球命中率, 篮板, 助攻, 抢断, 盖帽, 失误, 犯规, 得分)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """
        # 编码为 UTF-8
        encoded_data = [
            [d.encode('utf-8').decode('utf-8') if isinstance(d, str) else d for d in row]
            for row in data
        ]
        cursor.executemany(sql, encoded_data)
        conn.commit()
        print(f"{table_name} 数据插入成功")
    except Exception as e:
        print(f"{table_name} 数据插入失败: {e}")
        conn.rollback()

if __name__ == "__main__":
    # 测试数据库连接
    if not test_db_connection():
        print("数据库连接测试失败，请检查连接信息。")
        exit()

    # 连接数据库
    conn_str = f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={server};DATABASE={database};UID={username};PWD={password};CHARSET=UTF8'
    conn = pyodbc.connect(conn_str)

    # # 创建 player_mapping 表（如果不存在）
    # cursor = conn.cursor()
    # # 建表语句修改
    # sql = """
    # IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'player_mapping')
    # BEGIN
    #     CREATE TABLE player_mapping (
    #         player_id INT IDENTITY(1,1) PRIMARY KEY,
    #         player_name NVARCHAR(255) COLLATE Chinese_PRC_CI_AS
    #     )
    # END
    # """
    # cursor.execute(sql)
    # conn.commit()
    # print("表 player_mapping 创建成功")

    url = "https://nba.hupu.com/stats/players/stl"
    html_content = fetch_html(url)
    if html_content is not None:
        soup = BeautifulSoup(html_content, "html.parser")
        tds = soup.find_all("td", {"class": "left", "width": "142"})
        for td in tds:
            a_tag = td.find("a")
            if a_tag and a_tag.string and a_tag.get("href"):
                player_name = a_tag.string.strip()
                href = a_tag["href"]
                print(f"名字: {player_name}, 链接: {href}")
                player_url = href if href.startswith("http") else f"https://nba.hupu.com{href}"
                player_html = fetch_html(player_url)
                if player_html:
                    player_soup = BeautifulSoup(player_html, "html.parser")
                    trs = player_soup.find_all("tr", class_="color_font1 borders_btm")
                    player_data = []
                    for tr in trs:
                        tds_in_tr = tr.find_all("td")
                        if tds_in_tr:
                            first_td_text = tds_in_tr[0].get_text(strip=True)
                            if re.match(r"\d{2}/\d{2}", first_td_text):
                                row_data = [td.get_text(strip=True) for td in tds_in_tr]
                                player_data.append(row_data)

                    # 获取球员 ID
                    player_id = get_player_id(conn, player_name)
                    if player_id is None:
                        print(f"跳过球员 {player_name}，ID 获取失败")
                        continue
                    # 判断表是否已存在
                    table_name = f"player_{player_id}"
                    if table_exists(conn, table_name):
                       print(f"球员 {player_name} 的数据表已存在，跳过插入。")
                       continue

                    # 创建球员数据表
                    create_player_table(conn, player_id)

                    # 插入球员数据
                    if player_data:
                        insert_player_data(conn, player_id, player_data)
                    else:
                        print(f"球员 {player_name} 没有可插入的数据，跳过。")

                else:
                    print(f"无法获取球员详情页: {player_url}")
    else:
        print("获取网页内容失败，无法解析。")

    # 关闭数据库连接
    conn.close()