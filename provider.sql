--
-- PostgreSQL database dump
--

-- Dumped from database version 11.2 (Debian 11.2-1.pgdg90+1)
-- Dumped by pg_dump version 11.2 (Debian 11.2-1.pgdg90+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: bills; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.bills (
    id integer NOT NULL,
    user_id integer NOT NULL,
    cost numeric DEFAULT 0 NOT NULL,
    due_date timestamp without time zone DEFAULT now() NOT NULL,
    is_paid boolean DEFAULT false NOT NULL
);


--
-- Name: bills_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.bills_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: bills_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.bills_id_seq OWNED BY public.bills.id;


--
-- Name: services; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.services (
    id integer NOT NULL,
    name character varying(45) NOT NULL,
    description character varying(255),
    cost numeric DEFAULT 0 NOT NULL
);


--
-- Name: services_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.services_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: services_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.services_id_seq OWNED BY public.services.id;


--
-- Name: user_groups; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.user_groups (
    id integer NOT NULL,
    name character varying(45) NOT NULL
);


--
-- Name: user-groups_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public."user-groups_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: user-groups_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public."user-groups_id_seq" OWNED BY public.user_groups.id;


--
-- Name: users; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.users (
    id integer NOT NULL,
    nickname character varying(45) NOT NULL,
    full_name character varying(255),
    group_id integer NOT NULL,
    is_active boolean DEFAULT true NOT NULL,
    password character varying(255) NOT NULL
);


--
-- Name: users_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.users_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: users_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;


--
-- Name: users_to_services; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.users_to_services (
    user_id integer NOT NULL,
    service_id integer NOT NULL
);


--
-- Name: bills id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.bills ALTER COLUMN id SET DEFAULT nextval('public.bills_id_seq'::regclass);


--
-- Name: services id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.services ALTER COLUMN id SET DEFAULT nextval('public.services_id_seq'::regclass);


--
-- Name: user_groups id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_groups ALTER COLUMN id SET DEFAULT nextval('public."user-groups_id_seq"'::regclass);


--
-- Name: users id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);


--
-- Data for Name: bills; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.bills (id, user_id, cost, due_date, is_paid) FROM stdin;
\.


--
-- Data for Name: services; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.services (id, name, description, cost) FROM stdin;
11	1239302716	\N	794
13	1159654684	\N	2101
14	1716145102	\N	3665
15	790158410	\N	1857
16	2040375432	\N	1154
17	253061374	\N	4105
18	1436609765	\N	3839
19	1667965508	\N	2065
20	1554613674	\N	4305
21	1589752833	\N	822
22	1688157228	\N	2438
23	1717014310	\N	2352
24	1883178590	\N	3125
25	877407373	\N	4847
26	160116142	\N	1630
27	13145881	\N	2320
28	874951108	\N	640
29	266623401	\N	2953
30	1214590496	\N	632
31	558284143	\N	1667
32	2062708619	\N	4671
33	730538054	\N	633
34	1366035412	\N	2382
35	676852020	\N	3850
36	817179334	\N	4890
37	1101703205	\N	808
38	2122008466	\N	2205
39	763798042	\N	733
40	1225990479	\N	493
41	1141269083	\N	989
42	685227863	\N	2055
3	1110184397	g	389
4	102161346	5	1709
43	g	f	1
44	g	f	1
45	f	f	1
46	g	g	1
47	f	f	1
48	f	f	1
49	f	f	1
51	cleaning	just some cleaning	1500
50	a	a12	2
10	6144114		3669
5	626317541	no	1522
52	showing	show must go on	150000000
6	695692152	what is this?	3794
12	1302483727	not suited for kids	1915
\.


--
-- Data for Name: user_groups; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.user_groups (id, name) FROM stdin;
12	monk
\.


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.users (id, nickname, full_name, group_id, is_active, password) FROM stdin;
6	yuri	Simpson	12	t	pswd
1	prizrak9	Pavlo	12	f	noadmin
8	amakana	Alexander	12	t	pswd
5	prizrak	Memem	12	f	admin
\.


--
-- Data for Name: users_to_services; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.users_to_services (user_id, service_id) FROM stdin;
6	51
\.


--
-- Name: bills_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.bills_id_seq', 1, true);


--
-- Name: services_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.services_id_seq', 52, true);


--
-- Name: user-groups_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."user-groups_id_seq"', 4, true);


--
-- Name: users_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.users_id_seq', 8, true);


--
-- Name: bills bills_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.bills
    ADD CONSTRAINT bills_pkey PRIMARY KEY (id);


--
-- Name: services services_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.services
    ADD CONSTRAINT services_pkey PRIMARY KEY (id);


--
-- Name: user_groups user-groups_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.user_groups
    ADD CONSTRAINT "user-groups_pkey" PRIMARY KEY (id);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- Name: users_to_services users_to_services_user_id_service_id_pk; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users_to_services
    ADD CONSTRAINT users_to_services_user_id_service_id_pk PRIMARY KEY (user_id, service_id);


--
-- Name: bills_id_uindex; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX bills_id_uindex ON public.bills USING btree (id);


--
-- Name: services_id_uindex; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX services_id_uindex ON public.services USING btree (id);


--
-- Name: user-groups_id_uindex; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "user-groups_id_uindex" ON public.user_groups USING btree (id);


--
-- Name: users_id_uindex; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX users_id_uindex ON public.users USING btree (id);


--
-- Name: users_nickname_uindex; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX users_nickname_uindex ON public.users USING btree (nickname);


--
-- Name: bills bills_users_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.bills
    ADD CONSTRAINT bills_users_id_fk FOREIGN KEY (user_id) REFERENCES public.users(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: users_to_services users_to_services_services_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users_to_services
    ADD CONSTRAINT users_to_services_services_id_fk FOREIGN KEY (service_id) REFERENCES public.services(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: users_to_services users_to_services_users_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users_to_services
    ADD CONSTRAINT users_to_services_users_id_fk FOREIGN KEY (user_id) REFERENCES public.users(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: users users_user-groups_id_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT "users_user-groups_id_fk" FOREIGN KEY (group_id) REFERENCES public.user_groups(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

