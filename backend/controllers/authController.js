const User = require('../models/userModel');

exports.signup = async (req, res) => {
    const { id, password } = req.body;
    try {
        const existing = await User.findOne({ id });
        if (existing) return res.json({ success: false, message: '이미 등록된 아이디입니다.' });

        const user = new User({ id, password }); // 프로토타입에서는 해싱 생략
        await user.save();
        res.json({ success: true, message: '회원가입 완료!' });
    } catch (err) {
        res.json({ success: false, message: '서버 오류' });
    }
};

exports.login = async (req, res) => {
    const { id, password } = req.body;
    try {
        const user = await User.findOne({ id, password });
        if (!user) return res.json({ success: false, message: '등록되지 않은 계정입니다.' });

        res.json({ success: true, message: '로그인 성공!' });
    } catch (err) {
        res.json({ success: false, message: '서버 오류' });
    }
};
