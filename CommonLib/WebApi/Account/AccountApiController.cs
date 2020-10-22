using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Models.WebApi;
using CommonLib.Data;
using System.Web;
using System.Net.Http;
using CommonLib.Models.Data;
using CommonLib.WebApi.Account.Models;
using System.Web.Http;

namespace CommonLib.WebApi.Account
{
    public class AccountApiController : ApiControllerBase<M_LOGIN>
    {
        public AccountApiController()
        {
            Repository = new RepositoryOneLab<M_LOGIN>();
        }


        [HttpPost]
        public HttpResponseMessage CheckEmail(Login _login)
        {
            try
            {
                var login = Repository.Context.Set<M_LOGIN>().SingleOrDefault(_ => _.EMAIL == _login.Email.ToLower());
                if (login != null)
                {
                    return Request.CreateResponse<Login>(new Login
                    {
                        LoginId = login.LOGIN_ID,
                        LoginName = login.LOGIN_NAME,
                        Email = login.EMAIL,
                    });
                }
                return Request.CreateResponse<Login>(new Login { LoginId = -1 });
            }
            catch (Exception e)
            {
                return Request.CreateResponse<Login>(new Login { LoginId = -1, Email=e.Message });
            }

        }

        [HttpPost]
        public HttpResponseMessage CreateLogin(Login _login)
        {
            DateTime now = DateTime.Now;
            var login = Repository.Context.Set<M_LOGIN>().SingleOrDefault(_ => _.EMAIL == _login.Email.ToLower());
            if(login==null)
            {
                M_LOGIN m_LOGIN = new M_LOGIN
                {
                    PARTNER_ID = -1,
                    UUID = Guid.NewGuid(),
                    LOGIN_KIND_ID = -1,
                    MEDIA_KIND_ID = -1,
                    LOGIN_NAME = _login.LoginName,
                    PASSWORD_HASH = _login.Password,
                    REQUIRED_CHANGE_PASSWORD = false,
                    PASSWORD_CHANGED_TIME = now,
                    EMAIL = _login.Email.ToLower(),
                    EMAIL_CONFIRMED = false,
                    SMS_CONFIRMED = false,
                    EXTERNAL_CONFIRMED = false,
                    TWO_FACTOR_ENABLED = false,
                    LOCKOUT_ENABLED = false,
                    ACCESS_FAILED_COUNT = 0,
                    IS_ONLINE = false,
                    IS_ACTIVE = false,
                    LANGUAGE = "zh-tw",
                    CREATED_USER_ID = -1,
                    CREATED_TIME = now,
                    UPDATED_USER_ID = -1,
                    UPDATED_TIME=now
                };

                m_LOGIN = Repository.Context.Set<M_LOGIN>().Add(m_LOGIN);
                Repository.Context.SaveChanges();


                return Request.CreateResponse<Login>(new Login
                {
                    LoginId = m_LOGIN.LOGIN_ID,
                    LoginName = m_LOGIN.LOGIN_NAME,
                    Email = m_LOGIN.EMAIL
                });
            }
            return Request.CreateResponse<Login>(new Login
            {
                LoginId = login.LOGIN_ID,
                LoginName = login.LOGIN_NAME,
                Email = login.EMAIL
            });
        }

        public HttpResponseMessage AssociateLoginUser(User user)
        {
            DateTime now = DateTime.Now;

            var login = Repository.Context.Set<M_LOGIN>().SingleOrDefault(_ => _.LOGIN_ID  == user.LoginId);
            if(login!=null)
            {
                M_USER m_USER = Repository.Context.Set<M_USER>()
                    .SingleOrDefault(_ => _.LOGIN_ID == login.LOGIN_ID && _.LOGIN_PROVIDER == (int)user.Provider);

                if(m_USER==null)
                {
                    m_USER = new M_USER
                    {
                        LOGIN_ID = login.LOGIN_ID,
                        ID = user.Id,
                        TOKEN = user.Token,
                        REFRESH_TOKEN = user.RefreshToken,
                        EXPIRES_IN = now, //user.ExpiresIn,
                        NAME = user.Name,
                        NICK_NAME = user.NickName,
                        LAST_NAME = user.LastName,
                        EMAIL = login.EMAIL,
                        GENDER = user.Gender,
                        BIRTHDAY = now, //user.Birthday,
                        PICTURE_URL = user.PictureUrl,
                        LOGIN_PROVIDER = (int)user.Provider
                    };

                    Repository.Context.Set<M_USER>().Add(m_USER);
                    Repository.Context.SaveChanges();
                }
                return Request.CreateResponse<User>(user);
            }
            return Request.CreateResponse<User>(new User
            {
                LoginId = -1
            });
        }
    }
}
