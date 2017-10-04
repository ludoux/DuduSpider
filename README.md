能跑起来的！能用的！
---
**.NET CORE 2.0 程序，额外引用 NuGet 依赖项```Newtonsoft.Json(10.0.3)```**

对于知乎读读日报的抓取推送程序，理论上跨平台

## Screenshots

![Screenshot@Kindle](https://raw.githubusercontent.com/ludoux/DuduSpider/master/Pic/Screenshot@Kindle.png)

![Article Screenshot@Kindle](https://raw.githubusercontent.com/ludoux/DuduSpider/master/Pic/Article-Screenshot@Kindle.png)

## 进展

- [x] 抓取“热门文章”
- [x] 下载网页及图片，去除样式和脚本
- [x] 使用 KindleGen 生成 Guardian 样式的电子书
- [x] 带 Authorization 抓取用户个性化推荐后的首页
- [x] 通过邮件推送到 Kindle，校对样式
- [x] 将进展写入磁盘文件
- [x] 定时更新，将新的故事添加进队列
- [x] 小范围测试
- [x] 带命令启动，分为首次/更新/推送三种启动模式
- [x] 带启动参数运行程序（```-f "authorization string"```，第一个参数支持```-f(F)``` ```-u(U)``` ```-l(L)```分别对应三种启动模式）
- [ ] 填坑（特别是 Manifestx 有关的）
- [ ] 测试上述操作
- [ ] 为部署做准备，性能优化，异常捕获，容错率
- [ ] 个人部署测试
- [ ] 个人部署上线

## 坑

1. 目前文件为防止重名和非法字符，文件名均为网络位置文件名或故事标题的 MD5_16 值
2. 下载图片时，若网页上图片总数大于 8，就不下载此网页和图片，而且直接写 Manifest，后面处理会去掉返回个```notEmptyStoryList```
3. 非知乎链接，在代码中写死了（```List<Uri> allowedUrlHost```）只接受微信公众号（mp.weixin.qq.com），其它直接直接写 Manifest，后面处理会去掉返回个```notEmptyStoryList```
4. Url 的相对绝对和“//”开头的，处理有问题（过于生硬），应该要专门写一个 Url 格式化的方法
5. ContentType 目前还是手动指定后缀名（没有处理 .jpeg 链接的图片资源）
6. 知乎站内附有 css，读取但在 html 文件填写方法中，将 css 的相关写入操作注释掉了
7. Manifest 中 .html 和图片资源目前路径是相对于“根”目录的相对路径，在 .opf 文件操作中直接写入，在 contents.html 文件中硬替换路径到所需的相对路径
8. 注意正反斜杠！xml 文件相关均用```/```
9. .ncx 文件中的各文章中没有 description 和 author 字段
10. .ncx 文件中网格模式头部 LOGO 图片路径文件名和后缀名写死
11. 抓取首页流返回故事数只有最近的三四十余篇
12. 由文件导入进度时，若仅下载最新 html 文件，是```List<Cell>```。若仅制作电子书，是```List<Manifestx> manifest```
13. ```Manifestx```的导入导出十分恶心（优先级放低，性能优化时再考虑，估计要改类架构）
14. 先下载首页流，当热门流中出现相同文章 ID 时，去除热门队列

## 伪.TODO

1. 用 .yml 文件来存储像登陆凭证，邮箱地址之类的设置
2. ContentType 希望用键-值类型来存储，两个方向各一个（要为 .jpeg/.jpg 这种异端的存在考虑

## LICENSE

MIT